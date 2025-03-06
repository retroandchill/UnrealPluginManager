using System.IO.Abstractions;
using System.IO.Compression;
using System.Text.Json;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Mappers;
using UnrealPluginManager.Core.Model.Engine;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Resolution;
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

  /// <inheritdoc />
  public async Task<Option<PluginVersionInfo>> GetPluginVersionInfo(string pluginName, SemVersion version) {
    return await _dbContext.PluginVersions
        .Where(x => x.Parent.Name == pluginName && x.VersionString == version.ToString())
        .ToPluginVersionInfo()
        .FirstOrDefaultAsync();
  }

  /// <inheritdoc />
  public async Task<Option<PluginVersionInfo>> GetPluginVersionInfo(string pluginName, SemVersionRange versionRange) {
    return await _dbContext.PluginVersions
        .Where(x => x.Parent.Name == pluginName)
        .WhereVersionInRange(versionRange)
        .OrderByVersionDecending()
        .ToPluginVersionInfo()
        .FirstOrDefaultAsync();
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
  public async Task<ResolutionResult> GetDependencyList(string pluginName, SemVersionRange? targetVersion = null) {
    var plugin = await _dbContext.PluginVersions
        .Include(p => p.Parent)
        .Include(p => p.Dependencies)
        .Where(p => p.Parent.Name == pluginName)
        .WhereVersionInRange(targetVersion ?? SemVersionRange.AllRelease)
        .OrderByVersionDecending()
        .ToPluginVersionInfo()
        .FirstAsync();

    var dependencyList = await GetPossibleVersions(plugin.Dependencies);
    if (dependencyList.UnresolvedDependencies.Count > 0) {
      throw new DependencyResolutionException(
          $"Unable to resolve plugin names:\n{string.Join("\n", dependencyList.UnresolvedDependencies)}");
    }

    return GetDependencyList(plugin, dependencyList);
  }

  /// <inheritdoc />
  public ResolutionResult GetDependencyList(IDependencyChainNode root, DependencyManifest manifest) {
    var formula = ExpressionSolver.Convert(root, manifest.FoundDependencies);
    return formula.Solve()
        .Right(ResolutionResult (r) => r)
        .Left(l => GetDependencyList(l, root, manifest));
  }

  private static ResolutionResult GetDependencyList(List<SelectedVersion> selectedVersions, IDependencyChainNode root, DependencyManifest manifest) {
    IEnumerable<PluginVersionInfo> result;
    if (root is PluginVersionInfo plugin) {
      result = selectedVersions
          .Select(p => p.Name == root.Name
                      ? plugin
                      : manifest.FoundDependencies[p.Name].First(d => d.Version == p.Version));
    } else {
      result = selectedVersions
          .Where(p => p.Name != root.Name)
          .Select(p => manifest.FoundDependencies[p.Name].First(d => d.Version == p.Version));
    }

    return result.Select(p => p.ToPluginSummary())
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
    var archive = new ZipArchive(fileData);
    var pluginDescriptorFile = archive.Entries
        .FirstOrDefault(x => x.Name.EndsWith(".uplugin"));
    if (pluginDescriptorFile is null) {
      throw new BadSubmissionException("Uplugin file was not found");
    }

    var baseName = Path.GetFileNameWithoutExtension(pluginDescriptorFile.FullName);

    PluginDescriptor descriptor;
    try {
      await using var upluginFile = pluginDescriptorFile.Open();
      var descriptorOut = await JsonSerializer.DeserializeAsync<PluginDescriptor>(upluginFile, JsonOptions);
      ArgumentNullException.ThrowIfNull(descriptorOut);
      descriptor = descriptorOut;
    } catch (JsonException e) {
      throw new BadSubmissionException("Uplugin file was malformed", e);
    }

    var exists = await _dbContext.PluginVersions.AnyAsync(x => x.Parent.Name == baseName
                                                               && x.VersionString == descriptor.VersionName.ToString());
    if (exists) {
      throw new BadSubmissionException($"Plugin {baseName} version {descriptor.VersionName} already exists");
    }

    var partitionedPlugin = await _pluginStructureService.PartitionPlugin(baseName, descriptor.VersionName,
                                                                          engineVersion, archive);
    return await AddPlugin(baseName, descriptor, new EngineFileData(engineVersion, partitionedPlugin));
  }

  /// <inheritdoc />
  public async Task<PluginDetails> SubmitPlugin(IDirectoryInfo pluginDirectory, string engineVersion) {
    var pluginDescriptorFile = pluginDirectory.GetFiles("*.uplugin", SearchOption.TopDirectoryOnly)
        .FirstOrDefault();
    if (pluginDescriptorFile is null) {
      throw new BadSubmissionException("Uplugin file was not found");
    }

    var baseName = Path.GetFileNameWithoutExtension(pluginDescriptorFile.FullName);

    PluginDescriptor descriptor;
    try {
      await using var upluginFile = pluginDescriptorFile.OpenRead();
      var descriptorOut = await JsonSerializer.DeserializeAsync<PluginDescriptor>(upluginFile, JsonOptions);
      ArgumentNullException.ThrowIfNull(descriptorOut);
      descriptor = descriptorOut;
    } catch (JsonException e) {
      throw new BadSubmissionException("Uplugin file was malformed", e);
    }

    var exists = await _dbContext.PluginVersions.AnyAsync(x => x.Parent.Name == baseName
                                                               && x.VersionString == descriptor.VersionName.ToString());
    if (exists) {
      throw new BadSubmissionException($"Plugin {baseName} version {descriptor.VersionName} already exists");
    }

    var partitionedPlugin = await _pluginStructureService.PartitionPlugin(baseName, descriptor.VersionName,
                                                                          engineVersion, pluginDirectory);
    return await AddPlugin(baseName, descriptor, new EngineFileData(engineVersion, partitionedPlugin));
  }

  /// <inheritdoc />
  public async Task<IFileInfo> GetPluginSource(string pluginName, SemVersion targetVersion) {
    var plugin = await _dbContext.PluginVersions
        .Include(p => p.Parent)
        .Where(p => p.Parent.Name == pluginName)
        .Where(p => p.VersionString == targetVersion.ToString())
        .FirstOrDefaultAsync();
    if (plugin is null) {
      throw new PluginNotFoundException($"Plugin {pluginName} was not found!");
    }

    return _storageService.RetrievePluginSource(pluginName, plugin.Version)
        .OrElseThrow(() => new PluginNotFoundException($"Source for {pluginName} was not found!"));
  }

  /// <inheritdoc />
  public async Task<IFileInfo> GetPluginBinaries(string pluginName, SemVersion targetVersion, string engineVersion,
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

    return _storageService.RetrievePluginBinaries(pluginName, plugin.Parent.Version, engineVersion, platform)
        .OrElseThrow(() => new PluginNotFoundException($"Binaries for {pluginName} was not found!"));
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

  /// <inheritdoc />
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

  /// <inheritdoc/>
  public async Task<Stream> GetPluginFileData(string pluginName, SemVersion targetVersion, string engineVersion,
                                              IReadOnlyCollection<string> targetPlatforms) {
    var fileStream = _fileSystem.FileStream.New(Path.Join(Path.GetTempPath(), Path.GetRandomFileName()),
                                                FileMode.Create, FileAccess.ReadWrite,
                                                FileShare.Read, 4096, FileOptions.DeleteOnClose);

    try {
      using var destinationZip = new ZipArchive(fileStream, ZipArchiveMode.Create, true);
      await using (var source = await GetPluginSource(pluginName, targetVersion).Map(x => x.OpenRead())) {
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

  /// <inheritdoc />
  public async IAsyncEnumerable<IFileInfo> GetAllPluginData(string pluginName, SemVersion targetVersion, string engineVersion,
                                                            IReadOnlyCollection<string> targetPlatforms) {
    yield return await GetPluginSource(pluginName, targetVersion);

    await foreach (var (_, archive) in GetPluginBinaries(pluginName, targetVersion, engineVersion, targetPlatforms)) {
      yield return archive;
    }
  }

  /// <inheritdoc />
  public async IAsyncEnumerable<IFileInfo> GetAllPluginData(string pluginName, SemVersionRange targetVersion,
                                                            string engineVersion,
                                                            IReadOnlyCollection<string> targetPlatforms) {
    var latestVersion = await _dbContext.PluginVersions
        .Where(p => p.Parent.Name == pluginName)
        .WhereVersionInRange(targetVersion)
        .OrderByVersionDecending()
        .Select(x => x.VersionString)
        .FirstOrDefaultAsync()
        .ToOptionAsync()
        .Map(x => x.OrElseThrow(() => new PluginNotFoundException($"Plugin {pluginName} was not found!")))
        .Map(x => SemVersion.Parse(x));

    yield return await GetPluginSource(pluginName, latestVersion);

    await foreach (var (_, archive) in GetPluginBinaries(pluginName, latestVersion, engineVersion, targetPlatforms)) {
      yield return archive;
    }
  }
}