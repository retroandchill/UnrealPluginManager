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
    public async Task<List<PluginSummary>> GetDependencyList(string pluginName) {
        var plugin = await _dbContext.PluginVersions
            .Include(p => p.Parent)
            .Include(p => p.Dependencies)
            .Where(p => p.Parent.Name == pluginName)
            .OrderByDescending(p => p.VersionString)
            .FirstAsync();

        var pluginData = new Dictionary<string, List<PluginVersion>> { { plugin.Parent.Name, [plugin] } };
        var unresolved = new HashSet<string>();

        unresolved.UnionWith(plugin.Dependencies
            .Where(x => x.Type == PluginType.Provided)
            .Select(pd => pd.PluginName));
        while (unresolved.Count > 0) {
            var currentlyExisting = unresolved.ToHashSet();
            var plugins = (await _dbContext.PluginVersions
                    .Include(p => p.Parent)
                    .Include(p => p.Dependencies)
                    .Where(p => unresolved.Contains(p.Parent.Name))
                    .OrderByDescending(p => p.VersionString)
                    .ToListAsync())
                .GroupBy(x => x.Parent.Name);

            foreach (var pluginList in plugins) {
                unresolved.Remove(pluginList.Key);
                var asList = pluginList
                    .OrderByDescending(p => p.Version, SemVersion.PrecedenceComparer)
                    .ToList();
                pluginData.Add(pluginList.Key, asList);

                unresolved.UnionWith(pluginList
                    .SelectMany(p => p.Dependencies)
                    .Where(x => x.Type == PluginType.Provided && !pluginData.ContainsKey(x.PluginName))
                    .Select(pd => pd.PluginName));
            }

            var intersection = currentlyExisting.Intersect(unresolved).ToList();
            if (intersection.Count > 0) {
                throw new DependencyResolutionException(
                    $"Unable to resolve plugin names:\n{string.Join("\n", intersection)}");
            }
        }

        var formula = ExpressionSolver.Convert(pluginName, plugin.Version, pluginData);
        var bindings = formula.Solve();
        if (bindings.IsNone) {
            return [];
        }

        return bindings.SelectMany(b => b)
            .Select(p => pluginData[p.Item1].First(d => d.Version == p.Item2))
            .Select(p => p.ToPluginSummary())
            .ToList();
    }

    /// <inheritdoc/>
    public async Task<PluginDetails> AddPlugin(string pluginName, PluginDescriptor descriptor,
        EngineFileData? storedFile = null) {
        var plugin = await _dbContext.Plugins
            .Where(x => x.Name == pluginName)
            .FirstOrDefaultAsync();
        if (plugin is null) {
            plugin = descriptor.ToPlugin(pluginName);
            _dbContext.Plugins.Add(plugin);
        }
        var pluginVersion = descriptor.ToPluginVersion(); 
        pluginVersion.ParentId = plugin.Id;
        pluginVersion.Binaries.AddRange(storedFile.ToPluginBinaries());
        _dbContext.PluginVersions.Add(pluginVersion);
        await _dbContext.SaveChangesAsync();
        return plugin.ToPluginDetails(pluginVersion);
    }

    /// <inheritdoc/>
    public async Task<PluginDetails> SubmitPlugin(Stream fileData, Version engineVersion) {
        var fileInfo = await _storageService.StorePlugin(fileData);
        await using var storedData = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
        using var archive = new ZipArchive(storedData);

        var archiveEntry = archive.Entries
            .FirstOrDefault(entry => entry.FullName.EndsWith(".uplugin"));
        if (archiveEntry is null) {
            throw new BadSubmissionException("Uplugin file was not found");
        }

        var baseName = Path.GetFileNameWithoutExtension(archiveEntry.FullName);
        await using var upluginFile = archiveEntry.Open();

        try {
            var descriptor = await JsonSerializer.DeserializeAsync<PluginDescriptor>(upluginFile, JsonOptions);
            if (descriptor is null) {
                throw new BadSubmissionException("Uplugin file was malformed");
            }

            return await AddPlugin(baseName, descriptor, new EngineFileData(engineVersion, fileInfo));
        } catch (JsonException e) {
            throw new BadSubmissionException("Uplugin file was malformed", e);
        }
    }

    /// <inheritdoc/>
    public async Task<Stream> GetPluginFileData(string pluginName, SemVersionRange targetVersion, string engineVersion) {
        var pluginInfo = await _dbContext.UploadedPlugins
            .Include(x => x.Parent)
            .Include(x => x.Parent.Parent)
            .Where(p => p.Parent.Parent.Name == pluginName)
            .OrderByDescending(p => p.Parent.VersionString)
            .Where(x => x.EngineVersion == engineVersion)
            .AsAsyncEnumerable()
            .Where(p => p.Parent.Version.Satisfies(targetVersion))
            .FirstOrDefaultAsync();
        if (pluginInfo == null) {
            throw new PluginNotFoundException($"Plugin '{pluginName}' not found.");
        }

        return _storageService.RetrievePlugin(pluginInfo.FilePath);
    }
}