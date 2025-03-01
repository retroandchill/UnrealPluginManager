using System.IO.Abstractions;
using System.IO.Compression;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Mappers;
using UnrealPluginManager.Core.Model.Engine;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Storage;
using UnrealPluginManager.Core.Pagination;
using UnrealPluginManager.Core.Solver;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Provides operations for managing plugins within the Unreal Plugin Manager application.
/// </summary>
[AutoConstructor]
public partial class PluginService : IPluginService {
    private readonly UnrealPluginManagerContext _dbContext;
    private readonly IStorageService _storageService;
    private readonly IFileSystem _fileSystem;
    private readonly IPluginStructureService _pluginStructureService;

    private static readonly JsonSerializerOptions JsonOptions = new() {
            AllowTrailingCommas = true
    };

    /// <param name="matcher"></param>
    /// <param name="pageable"></param>
    /// <inheritdoc/>
    public Task<Page<PluginOverview>> ListPlugins(string matcher = "*", Pageable pageable = default) {
        return _dbContext.Plugins
                .Where(x => EF.Functions.Like(x.Name, matcher.Replace("*", "%")))
                .OrderByDescending(x => x.Name)
                .ToPluginOverview()
                .ToPageAsync(pageable);
    }

    /// <inheritdoc/>
    public Task<List<PluginVersionInfo>> RequestPluginInfos(List<PluginVersionRequest> requestedVersions) {
        return _dbContext.PluginVersions
                .Where(x => requestedVersions.Contains(new PluginVersionRequest(x.Parent.Name, x.Version)))
                .ToPluginVersionInfo()
                .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<DependencyManifest> GetPossibleVersions(List<PluginDependency> dependencies) {
        var manifest = new DependencyManifest();
        var unresolved = dependencies
                .Where(x => x.Type == PluginType.Provided)
                .Select(pd => pd.PluginName)
                .ToHashSet();

        while (unresolved.Count > 0) {
            var currentlyExisting = unresolved.ToHashSet();
            var plugins = (await _dbContext.PluginVersions
                            .Include(p => p.Parent)
                            .Include(p => p.Dependencies)
                            .Where(p => unresolved.Contains(p.Parent.Name))
                            .OrderByVersionDecending()
                            .ToPluginVersionInfo()
                            .ToListAsync())
                    .GroupBy(x => x.Name);

            foreach (var pluginList in plugins) {
                unresolved.Remove(pluginList.Key);
                var asList = pluginList
                        .OrderByDescending(p => p.Version, SemVersion.PrecedenceComparer)
                        .ToList();
                manifest.FoundDependencies.Add(pluginList.Key, asList);

                unresolved.UnionWith(pluginList
                                             .SelectMany(p => p.Dependencies)
                                             .Where(x => x.Type == PluginType.Provided &&
                                                         !manifest.FoundDependencies.ContainsKey(x.PluginName))
                                             .Select(pd => pd.PluginName));
            }

            var intersection = currentlyExisting.Intersect(unresolved).ToList();
            if (intersection.Count <= 0) {
                continue;
            }

            manifest.UnresolvedDependencies.AddRange(intersection);
            unresolved.RemoveWhere(x => intersection.Contains(x));
        }

        return manifest;
    }

    /// <inheritdoc/>
    public async Task<List<PluginSummary>> GetDependencyList(string pluginName) {
        var plugin = await _dbContext.PluginVersions
                .Include(p => p.Parent)
                .Include(p => p.Dependencies)
                .Where(p => p.Parent.Name == pluginName)
                .OrderByVersionDecending()
                .ToPluginVersionInfo()
                .FirstAsync();

        var dependencyList = await GetPossibleVersions(plugin.Dependencies);
        if (dependencyList.UnresolvedDependencies.Count > 0) {
            throw new DependencyResolutionException(
                                                    $"Unable to resolve plugin names:\n{string.Join("\n", dependencyList.UnresolvedDependencies)}");
        }

        dependencyList.FoundDependencies.Add(pluginName, [plugin]);
        var formula = ExpressionSolver.Convert(pluginName, plugin.Version, dependencyList.FoundDependencies);
        var bindings = formula.Solve();
        if (bindings.IsNone) {
            return [];
        }

        return bindings.SelectMany(b => b)
                .Select(p => p.Name == pluginName
                                ? plugin
                                : dependencyList.FoundDependencies[p.Name].First(d => d.Version == p.Version))
                .Select(p => p.ToPluginSummary())
                .ToList();
    }

    /// <inheritdoc/>
    public async Task<PluginDetails> AddPlugin(string pluginName, PluginDescriptor descriptor,
                                               EngineFileData? storedFile = null) {
        await using var transaction = await _dbContext.Database.BeginTransactionAsync();
        var plugin = await _dbContext.Plugins
                .Where(x => x.Name == pluginName)
                .FirstOrDefaultAsync();
        if (plugin is null) {
            plugin = descriptor.ToPlugin(pluginName);
            _dbContext.Plugins.Add(plugin);
            await _dbContext.SaveChangesAsync();
        }
        var pluginVersion = descriptor.ToPluginVersion();
        pluginVersion.ParentId = plugin.Id;
        pluginVersion.Binaries.AddRange(storedFile.ToPluginBinaries());
        _dbContext.PluginVersions.Add(pluginVersion);
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
        return plugin.ToPluginDetails(pluginVersion);
    }

    /// <inheritdoc/>
    public async Task<PluginDetails> SubmitPlugin(Stream fileData, string engineVersion) {
        var fileInfo = await _storageService.StorePlugin(fileData);
        await using var storedData = fileInfo.ZipFile.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        using var directoryHandle = _fileSystem.CreateDisposableDirectory(out var tempDir);
        using (var archive = new ZipArchive(storedData)) {
            await _fileSystem.ExtractZipFile(archive, tempDir.FullName);
        }

        var pluginDescriptorFile = tempDir.EnumerateFiles("*.uplugin")
                .FirstOrDefault();
        if (pluginDescriptorFile is null) {
            throw new BadSubmissionException("Uplugin file was not found");
        }

        var baseName = Path.GetFileNameWithoutExtension(pluginDescriptorFile.FullName);

        await using var upluginFile = pluginDescriptorFile.OpenRead();
        try {
            var descriptor = await JsonSerializer.DeserializeAsync<PluginDescriptor>(upluginFile, JsonOptions);
            ArgumentNullException.ThrowIfNull(descriptor);

            var partitionedPlugin = await _pluginStructureService.PartitionPlugin(baseName, descriptor.VersionName,
                engineVersion, tempDir);

            return await AddPlugin(baseName, descriptor, new EngineFileData(engineVersion, partitionedPlugin));
        } catch (JsonException e) {
            throw new BadSubmissionException("Uplugin file was malformed", e);
        }
    }

    /// <inheritdoc />
    public async Task<Stream> GetPluginSource(string pluginName, SemVersion targetVersion) {
        var plugin = await _dbContext.PluginVersions
                .Include(p => p.Parent)
                .Where(p => p.Parent.Name == pluginName)
                .Where(p => p.VersionString == targetVersion.ToString())
                .FirstOrDefaultAsync();
        if (plugin is null) {
            throw new PluginNotFoundException($"Plugin {pluginName} was not found!");
        }
        
        var fileInfo = _storageService.RetrievePluginSource(pluginName, plugin.Version)
                .OrElseThrow(() => new PluginNotFoundException($"Source for {pluginName} was not found!"));
        return fileInfo.OpenRead();
    }

    /// <inheritdoc />
    public async Task<Stream> GetPluginBinaries(string pluginName, SemVersion targetVersion, string engineVersion,
                                                string platform) {
        var plugin = await _dbContext.PluginBinaries
                .Include(b => b.Parent)
                .ThenInclude(v => v.Parent)
                .Where(b => b.Parent.Parent.Name == pluginName)
                .Where(b => b.Parent.VersionString == targetVersion.ToString())
                .Where(b => b.EngineVersion == engineVersion)
                .Where(b => b.Platform == platform)
                .FirstOrDefaultAsync();
        if (plugin is null) {
            throw new PluginNotFoundException($"Binaries for plugin {pluginName} was not found!");
        }
        
        var fileInfo = _storageService.RetrievePluginBinaries(pluginName, plugin.Parent.Version, engineVersion, platform)
                .OrElseThrow(() => new PluginNotFoundException($"Binaries for {pluginName} was not found!"));
        return fileInfo.OpenRead();
    }

    public async Task<Stream> GetPluginFileData(string pluginName, SemVersionRange targetVersion, string engineVersion,
                                          IReadOnlyCollection<string> targetPlatforms) {
        
        var latestVersion = await _dbContext.PluginVersions
                .Where(p => p.Parent.Name == pluginName)
                .WhereVersionInRange(targetVersion)
                .OrderByVersionDecending()
                .Select(x => x.VersionString)
                .FirstOrDefaultAsync();
        if (latestVersion is null) {
            throw new PluginNotFoundException($"Plugin {pluginName} was not found!");
        }
        
        return await GetPluginFileData(pluginName, SemVersion.Parse(latestVersion), engineVersion, targetPlatforms);
    }

    private async IAsyncEnumerable<(string, IFileInfo)> GetPluginBinaries(string pluginName, SemVersion targetVersion,
                                                                          string engineVersion,
                                                                          IReadOnlyCollection<string> targetPlatforms) {
        var binaries = await _dbContext.PluginBinaries
                .Include(b => b.Parent)
                .ThenInclude(v => v.Parent)
                .Where(b => b.Parent.Parent.Name == pluginName)
                .Where(b => b.Parent.VersionString == targetVersion.ToString())
                .Where(b => b.EngineVersion == engineVersion)
                .Where(b => targetPlatforms.Contains(b.Platform))
                .ToListAsync();
        if (binaries.Count != targetPlatforms.Count) {
            throw new PluginNotFoundException($"All binaries for plugin {pluginName} was not found!");
        }
        
        foreach (var binary in binaries) {
            var fileInfo = _storageService
                    .RetrievePluginBinaries(pluginName, binary.Parent.Version, engineVersion, binary.Platform)
                    .OrElseThrow(() => new PluginNotFoundException($"Binaries for {binary.Platform} was not found!"));
            yield return (binary.Platform, fileInfo);
        }
    }

    /// <inheritdoc/>
    public async Task<Stream> GetPluginFileData(string pluginName, SemVersion targetVersion, string engineVersion,
                                                IReadOnlyCollection<string> targetPlatforms) {
        var fileStream = _fileSystem.FileStream.New(Path.GetTempFileName(), FileMode.Create, FileAccess.ReadWrite, 
                                                    FileShare.Read, 4096, FileOptions.DeleteOnClose);
        
        try {
            using var destinationZip = new ZipArchive(fileStream);
            await using (var source = await GetPluginSource(pluginName, targetVersion)) {
                using var zipArchive = new ZipArchive(source);
                await destinationZip.Merge(zipArchive);
            }
            
            await foreach (var (_, archive) in GetPluginBinaries(pluginName, targetVersion, engineVersion, targetPlatforms)) {
                await using var binaries = archive.OpenRead();
                using var zipArchive = new ZipArchive(binaries);
                await destinationZip.Merge(zipArchive);
            }
        } catch (Exception) {
            // If we encounter an exception we need to close the file stream so that the temp file is properly deleted
            fileStream.Close();
            throw;
        }
        
        return fileStream;
    }
}