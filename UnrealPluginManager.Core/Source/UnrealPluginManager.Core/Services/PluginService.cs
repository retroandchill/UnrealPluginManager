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

public record RetrievedBinaryInformation(string Name, string Version, List<string> Platforms);


/// <summary>
/// Provides operations for managing plugins within the Unreal Plugin Manager application.
/// </summary>
[AutoConstructor]
public partial class PluginService : IPluginService {
  private readonly UnrealPluginManagerContext _dbContext;
  private readonly IStorageService _storageService;
  private readonly IFileSystem _fileSystem;
  private readonly IPluginStructureService _pluginStructureService;
  private readonly IJsonService _jsonService;

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

  public Task<Page<PluginVersionInfo>> ListLatestedVersions(string pluginName, SemVersionRange versionRange, Pageable pageable = default) {
    return _dbContext.PluginVersions
        .Where(x => EF.Functions.Like(x.Parent.Name, pluginName.Replace("*", "%")))
        .WhereVersionInRange(versionRange)
        .GroupBy(x => x.ParentId)
        .Select(x => x.OrderByDescending(v => v.Version, SemVersion.PrecedenceComparer).First())
        .ToPluginVersionInfo()
        .ToPageAsync(pageable);
  }

  /// <inheritdoc />
  public async Task<Option<PluginVersionInfo>> GetPluginVersionInfo(Guid pluginId, SemVersionRange versionRange) {
    return await _dbContext.PluginVersions
        .Where(x => x.ParentId == pluginId)
        .WhereVersionInRange(versionRange)
        .OrderByVersionDecending()
        .ToPluginVersionInfo()
        .FirstOrDefaultAsync();
  }

  public async Task<Option<PluginVersionInfo>> GetPluginVersionInfo(string pluginName, SemVersionRange versionRange) {
    return await _dbContext.PluginVersions
        .Where(x => x.Parent.Name == pluginName)
        .WhereVersionInRange(versionRange)
        .OrderByVersionDecending()
        .ToPluginVersionInfo()
        .FirstOrDefaultAsync();
  }

  public async Task<Option<PluginVersionInfo>> GetPluginVersionInfo(string pluginName, SemVersion version) {
    return await _dbContext.PluginVersions
        .Where(x => x.Parent.Name == pluginName)
        .Where(x => x.VersionString == version.ToString())
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
  public async Task<List<PluginSummary>> GetDependencyList(Guid pluginId, SemVersionRange? targetVersion = null) {
    var plugin = await _dbContext.PluginVersions
        .Where(p => p.ParentId == pluginId)
        .WhereVersionInRange(targetVersion ?? SemVersionRange.AllRelease)
        .OrderByVersionDecending()
        .ToPluginVersionInfo()
        .FirstAsync();

    var dependencyList = await GetPossibleVersions(plugin.Dependencies);
    return GetDependencyList(plugin, dependencyList);
  }

  /// <inheritdoc />
  public List<PluginSummary> GetDependencyList(IDependencyChainNode root, DependencyManifest manifest) {
    if (manifest.UnresolvedDependencies.Count > 0) {
      throw new MissingDependenciesException(manifest.UnresolvedDependencies);
    }
    var formula = ExpressionSolver.Convert(root, manifest.FoundDependencies);
    return formula.Solve()
        .Right(List<PluginSummary> (r) => throw new DependencyConflictException(r))
        .Left(l => GetDependencyList(l, root, manifest));
  }

  private static List<PluginSummary> GetDependencyList(List<SelectedVersion> selectedVersions, IDependencyChainNode root, DependencyManifest manifest) {
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
      descriptor = await _jsonService.DeserializeAsync<PluginDescriptor>(upluginFile);
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
      descriptor = await _jsonService.DeserializeAsync<PluginDescriptor>(upluginFile);
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
  public async Task<IFileInfo> GetPluginSource(Guid pluginId, Guid versionId) {
    var plugin = await _dbContext.PluginVersions
        .Include(p => p.Parent)
        .Where(p => p.ParentId == pluginId)
        .Where(p => p.VersionString == versionId.ToString())
        .FirstOrDefaultAsync();
    if (plugin is null) {
      throw new PluginNotFoundException($"Plugin {pluginId} was not found!");
    }

    return GetPluginSource(plugin.Parent.Name, plugin.Version);
  }

  private IFileInfo GetPluginSource(string pluginName, SemVersion version) {
    return _storageService.RetrievePluginSource(pluginName, version)
        .OrElseThrow(() => new PluginNotFoundException($"Source for {pluginName} was not found!"));
  }

  /// <inheritdoc />
  public async Task<IFileInfo> GetPluginBinaries(Guid pluginId, Guid versionId, string engineVersion,
                                                 string platform) {
    var plugin = await _dbContext.PluginBinaries
        .Include(b => b.Parent)
        .ThenInclude(v => v.Parent)
        .Where(b => b.Parent.ParentId == pluginId)
        .Where(b => b.ParentId == versionId)
        .Where(b => b.EngineVersion == engineVersion)
        .Where(b => b.Platform == platform)
        .FirstOrDefaultAsync();
    if (plugin is null) {
      throw new PluginNotFoundException($"Binaries for plugin {pluginId} was not found!");
    }

    return GetPluginBinaries(plugin.Parent.Parent.Name, plugin.Parent.Version, engineVersion, platform);
  }

  private IFileInfo GetPluginBinaries(string pluginName, SemVersion version, string engineVersion, string platform) {
    return _storageService.RetrievePluginBinaries(pluginName, version, engineVersion, platform)
        .OrElseThrow(() => new PluginNotFoundException($"Binaries for {pluginName} was not found!"));
  }

  private IEnumerable<(string, IFileInfo)> GetPluginBinaries(string pluginName, SemVersion version,
                                                             string engineVersion,
                                                             IReadOnlyCollection<string> targetPlatforms) {
    return targetPlatforms.Select(platform => (platform, _storageService
                                      .RetrievePluginBinaries(pluginName, version, engineVersion, platform)
                                      .OrElseThrow(() => new PluginNotFoundException($"Binaries for {platform} was not found!"))));
  }

  /// <inheritdoc />
  public async Task<Stream> GetPluginFileData(Guid pluginId, SemVersionRange targetVersion, string engineVersion,
                                              IReadOnlyCollection<string> targetPlatforms) {
    var latestVersion = await _dbContext.PluginVersions
        .Where(p => p.ParentId == pluginId)
        .WhereVersionInRange(targetVersion)
        .OrderByVersionDecending()
        .Select(x => x.Id)
        .Cast<Guid?>()
        .FirstOrDefaultAsync();
    if (latestVersion is null) {
      throw new PluginNotFoundException($"Plugin {pluginId} was not found!");
    }

    return await GetPluginFileData(pluginId, latestVersion.Value, engineVersion, targetPlatforms);
  }

  /// <inheritdoc/>
  public async Task<Stream> GetPluginFileData(Guid pluginId, Guid versionId, string engineVersion,
                                              IReadOnlyCollection<string> targetPlatforms) {
    var fileStream = _fileSystem.FileStream.New(Path.Join(Path.GetTempPath(), Path.GetRandomFileName()),
                                                FileMode.Create, FileAccess.ReadWrite,
                                                FileShare.Read, 4096, FileOptions.DeleteOnClose);

    try {
      using var destinationZip = new ZipArchive(fileStream, ZipArchiveMode.Create, true);
      await foreach (var archive in GetAllPluginData(pluginId, versionId, engineVersion, targetPlatforms)) {
        await using var data = archive.OpenRead();
        using var zipArchive = new ZipArchive(data);
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
  public async IAsyncEnumerable<IFileInfo> GetAllPluginData(Guid pluginId, Guid versionId, string engineVersion,
                                                            IReadOnlyCollection<string> targetPlatforms) {
    var binariesData = await RetrieveBinaryInformation(pluginId, versionId, engineVersion, targetPlatforms);
    var version = SemVersion.Parse(binariesData.Version);
    
    yield return GetPluginSource(binariesData.Name, version);
    foreach (var (_, archive) in GetPluginBinaries(binariesData.Name, version, engineVersion, targetPlatforms)) {
      yield return archive;
    }
  }

  /// <inheritdoc />
  public async IAsyncEnumerable<IFileInfo> GetAllPluginData(string pluginName, SemVersion pluginVersion, string engineVersion,
                                                            IReadOnlyCollection<string> targetPlatforms) {
    var binariesData = await _dbContext.PluginBinaries
        .Where(x => x.Parent.Parent.Name == pluginName)
        .Where(x => x.Parent.VersionString == pluginVersion.ToString())
        .Where(x => x.EngineVersion == engineVersion)
        .Where(x => targetPlatforms.Contains(x.Platform))
        .Select(x => x.Platform)
        .ToListAsync();
    
    var missing = binariesData.Except(targetPlatforms).ToList();
    if (missing.Count > 0) {
      throw new PluginNotFoundException($"Missing binaries for {string.Join(", ", missing)}");
    }
    
    yield return GetPluginSource(pluginName, pluginVersion);
    foreach (var (_, archive) in GetPluginBinaries(pluginName, pluginVersion, engineVersion, targetPlatforms)) {
      yield return archive;
    }
  }

  private async Task<RetrievedBinaryInformation> RetrieveBinaryInformation(Guid pluginId, Guid versionId, string engineVersion,
                                                                           IReadOnlyCollection<string> targetPlatforms) {
    var binariesData = await _dbContext.PluginVersions
        .Where(v => v.ParentId == pluginId)
        .Where(v => v.Id == versionId)
        .Select(v => new RetrievedBinaryInformation(
                    v.Parent.Name,
                    v.VersionString,
                    v.Binaries
                        .Where(x => x.EngineVersion == engineVersion)
                        .Where(x => targetPlatforms.Contains(x.Platform))
                        .Select(x => x.Platform)
                        .ToList()))
        .FirstOrDefaultAsync();
    if (binariesData is null) {
      throw new PluginNotFoundException($"Plugin {pluginId} was not found!");
    }
    
    var missing = binariesData.Platforms.Except(targetPlatforms).ToList();
    if (missing.Count > 0) {
      throw new PluginNotFoundException($"Missing binaries for {string.Join(", ", missing)}");
    }

    return binariesData;
  }

  /// <inheritdoc />
  public async IAsyncEnumerable<IFileInfo> GetAllPluginData(Guid pluginId, SemVersionRange targetVersion,
                                                            string engineVersion,
                                                            IReadOnlyCollection<string> targetPlatforms) {
    var latestVersion = await _dbContext.PluginVersions
        .Where(p => p.ParentId == pluginId)
        .WhereVersionInRange(targetVersion)
        .OrderByVersionDecending()
        .Select(x => new RetrievedBinaryInformation(x.Parent.Name, x.VersionString, 
                                                    x.Binaries
                                                        .Where(y => y.EngineVersion == engineVersion)
                                                        .Where(y => targetPlatforms.Contains(y.Platform))
                                                        .Select(y => y.Platform)
                                                        .ToList()))
        .FirstOrDefaultAsync()
        .ToOptionAsync()
        .Map(x => x.OrElseThrow(() => new PluginNotFoundException($"Plugin {pluginId} was not found!")));

    var version = SemVersion.Parse(latestVersion.Version);
    yield return GetPluginSource(latestVersion.Name, version);

    foreach (var (_, archive) in GetPluginBinaries(latestVersion.Name, version, engineVersion, targetPlatforms)) {
      yield return archive;
    }
  }
}