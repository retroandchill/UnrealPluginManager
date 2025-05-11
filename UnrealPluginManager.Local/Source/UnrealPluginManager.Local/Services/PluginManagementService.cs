using System.IO.Abstractions;
using LanguageExt;
using Retro.SimplePage;
using Semver;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Mappers;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Local.Exceptions;
using UnrealPluginManager.Local.Model.Cache;
using UnrealPluginManager.WebClient.Api;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// Service responsible for handling remote API calls related to plugin management.
/// </summary>
[AutoConstructor]
public partial class PluginManagementService : IPluginManagementService {
  private readonly IFileSystem _fileSystem;
  private readonly IRemoteService _remoteService;
  private readonly IEngineService _engineService;
  private readonly IPluginService _pluginService;
  private readonly IStorageService _storageService;
  private readonly ISourceDownloadService _sourceDownloadService;
  private readonly IBinaryCacheService _binaryCacheService;
  private readonly IJsonService _jsonService;

  private const int DefaultPageSize = 100;

  private const string PluginCacheDirectory = "Plugins";

  /// <inheritdoc />
  public async Task<Option<PluginBuildInfo>> FindLocalPlugin(string pluginName, SemVersion versionRange,
                                                             string engineVersion,
                                                             IReadOnlyCollection<string> platforms) {
    return await _binaryCacheService.GetCachedPluginBuild(pluginName, versionRange, engineVersion, platforms);
  }

  /// <inheritdoc />
  public Task<OrderedDictionary<string, Fin<List<PluginOverview>>>> GetPlugins(string searchTerm) {
    return _remoteService.GetApiAccessors<IPluginsApi>()
        .ToOrderedDictionaryAsync(async x => {
          try {
            return await searchTerm.ToEnumerable()
                .PageToEndAsync((y, p) => x.GetPluginsAsync(y, p.PageNumber, p.PageSize),
                    DefaultPageSize)
                .ToListAsync();
          } catch (ApiException e) {
            return Fin<List<PluginOverview>>.Fail(e);
          }
        });
  }

  /// <inheritdoc />
  public async Task<List<PluginOverview>> GetPlugins(string remote, string searchTerm) {
    var api = _remoteService.GetApiAccessor<IPluginsApi>(remote);
    return await searchTerm.ToEnumerable()
        .PageToEndAsync((y, p) => api.GetPluginsAsync(y, p.PageNumber, p.PageSize), DefaultPageSize)
        .ToListAsync();
  }

  /// <inheritdoc />
  public async Task<PluginVersionInfo> FindTargetPlugin(string pluginName, SemVersionRange versionRange,
                                                        string? engineVersion) {
    var currentlyInstalled = await _engineService.GetInstalledPluginVersion(pluginName, engineVersion)
        .MapAsync(x => x.SelectManyAsync(v => _pluginService.GetPluginVersionInfo(pluginName, v)));

    var resolved = await currentlyInstalled.OrElseAsync(() => LookupPluginVersion(pluginName, versionRange));
    return resolved.OrElseThrow(() =>
        new PluginNotFoundException($"Unable to resolve plugin {pluginName} with version {versionRange}."));
  }

  private async Task<Option<PluginVersionInfo>> LookupPluginVersion(string pluginName, SemVersionRange versionRange) {
    var cached = await _pluginService.GetPluginVersionInfo(pluginName, versionRange);

    return await cached.OrElseAsync(() => LookupPluginVersionOnCloud(pluginName, versionRange));
  }

  private async Task<Option<PluginVersionInfo>> LookupPluginVersionOnCloud(
      string pluginName, SemVersionRange versionRange) {
    var tasks = _remoteService.GetApiAccessors<IPluginsApi>()
        .Select(x => pluginName.ToEnumerable()
            .PageToEndAsync(
                (y, p) => x.Api.GetLatestVersionsAsync(y, versionRange.ToString(), p.PageNumber, p.PageSize),
                DefaultPageSize)
            .FirstOrDefaultAsync()
            .ToRef())
        .ToList();

    return await SafeTasks.WhenEach(tasks)
        .Where(version => !version.IsFaulted)
        .Select(x => x.Result)
        .FirstOrDefaultAsync(version => version is not null)
        .Map(x => x.ToOption());
  }

  /// <inheritdoc />
  public async Task<List<PluginSummary>> GetPluginsToInstall(IDependencyChainNode root, string? engineVersion) {
    var currentlyInstalled = await _engineService.GetInstalledPlugins(engineVersion)
        .ToDictionaryAsync(x => x.Name, x => x.Version);

    var allDependencies = root.Dependencies
        .Concat(currentlyInstalled.Select(x => new PluginDependency {
            PluginName = x.Key,
            PluginVersion = SemVersionRange.AtLeast(x.Value)
        }))
        .DistinctBy(x => x.PluginName)
        .ToList();

    var dependencyTasks = _pluginService.GetPossibleVersions(allDependencies)
        .Map(x => x.SetInstalledPlugins(currentlyInstalled)).ToEnumerable()
        .Concat(_remoteService.GetApiAccessors<IPluginsApi>()
            .Select((x, i) => x.Api.GetCandidateDependenciesAsync(allDependencies)
                .Map(y => y.SetRemoteIndex(i))))
        .ToList();

    var dependencyManifest = await SafeTasks.WhenEach(dependencyTasks)
        .Where(x => x.IsCompletedSuccessfully)
        .Select(x => x.Result)
        .Collapse();
    return _pluginService.GetDependencyList(root, dependencyManifest);
  }

  /// <inheritdoc />
  public async Task<PluginVersionInfo> UploadPlugin(string pluginName, SemVersion version, string? remote) {
    var plugin = await _pluginService.ListLatestVersions(pluginName, SemVersionRange.Equals(version))
        .Map(x => x.Count > 0 ? x[0] : null);
    if (plugin is null) {
      throw new PluginNotFoundException($"Unable to find plugin {pluginName} with version {version}.");
    }

    var remoteName = remote ?? _remoteService.DefaultRemoteName;
    if (remoteName is null) {
      throw new RemoteNotFoundException("No default remote configured.");
    }

    var manifest = plugin.ToPluginManifest();
    var patches = await _pluginService.GetSourcePatches(plugin.PluginId, plugin.VersionId)
        .Map(x => x.Select(y => y.Content)
            .ToList());

    await using var iconFile = plugin.Icon is not null
        ? _storageService.GetResourceStream(plugin.Icon.StoredFilename)
        : null;

    var readme = await _pluginService.GetPluginReadme(plugin.PluginId, plugin.VersionId)
        .ContinueWith(x => x.IsFaulted ? null : x.Result);

    var pluginsApi = _remoteService.GetApiAccessor<IPluginsApi>(remoteName);
    return await pluginsApi.SubmitPluginAsync(manifest, patches,
        iconFile is not null ? new FileParameter(iconFile) : null, readme);
  }

  /// <inheritdoc />
  public async Task<PluginBuildInfo> DownloadPlugin(string pluginName, SemVersion version,
                                                    int? remote, string engineVersion,
                                                    List<string> platforms) {
    var plugin = await _binaryCacheService.GetCachedPluginBuild(pluginName, version, engineVersion, platforms);
    return await plugin.OrElseGetAsync(async () => {
      if (!remote.HasValue) {
        throw new PluginNotFoundException(
            $"Unable to find plugin {pluginName} with version {version} in the local cache.");
      }

      var (remoteName, _) = _remoteService.GetAllRemotes().GetAt(remote.Value);
      var pluginsApi = _remoteService.GetApiAccessor<IPluginsApi>(remoteName);
      var pluginToDownload = await pluginName.ToEnumerable()
          .PageToEndAsync((y, p) => pluginsApi.GetLatestVersionsAsync(y, version.ToString(), p.PageNumber, p.PageSize),
              DefaultPageSize)
          .FirstOrDefaultAsync();
      if (pluginToDownload is null) {
        throw new PluginNotFoundException($"Unable to find plugin {pluginName} with version {version}.");
      }

      var manifest = pluginToDownload.ToPluginManifest();

      var patches = await pluginsApi.GetPluginPatchesAsync(pluginToDownload.PluginId, pluginToDownload.VersionId)
          .Map(x => x.Select(y => y.Content)
              .ToList());

      return await BuildFromManifest(manifest, patches, engineVersion, platforms);
    });
  }

  /// <inheritdoc />
  public async Task<PluginBuildInfo> BuildFromManifest(PluginManifest manifest, IReadOnlyList<string> patches,
                                                       string? engineVersion,
                                                       IReadOnlyCollection<string> platforms) {
    var installedEngine = _engineService.GetInstalledEngine(engineVersion);

    var pluginDirectoryName = Path.Join(_storageService.BaseDirectory,
        PluginCacheDirectory, manifest.Name, manifest.Version.ToString());
    var pluginDirectory = _fileSystem.DirectoryInfo.New(pluginDirectoryName);
    pluginDirectory.Create();

    var sourceDirectoryName = Path.Join(pluginDirectoryName, "Source");
    var sourceDirectory = _fileSystem.DirectoryInfo.New(sourceDirectoryName);
    sourceDirectory.Create();

    var upluginFile = sourceDirectory
        .EnumerateFiles("*.uplugin", SearchOption.AllDirectories)
        .FirstOrDefault();
    if (upluginFile is null) {
      await _sourceDownloadService.DownloadAndExtractSources(manifest.Source, sourceDirectory);

      upluginFile = sourceDirectory
          .EnumerateFiles("*.uplugin", SearchOption.AllDirectories)
          .FirstOrDefault();
      if (upluginFile is null) {
        throw new ContentNotFoundException("Missing a .uplugin file in the plugin's source directory.");
      }

      var rootDir = upluginFile.Directory.RequireNonNull();
      await _sourceDownloadService.PatchSources(rootDir, patches);
    }


    var buildDirectoryName = Path.Join(pluginDirectory.FullName, "Builds", Guid.CreateVersion7().ToString());
    var buildDirectory = _fileSystem.DirectoryInfo.New(buildDirectoryName);
    buildDirectory.Create();
    if (await _engineService.BuildPlugin(upluginFile, buildDirectory, engineVersion, platforms) != 0) {
      throw new InvalidOperationException();
    }

    var cachedPlugin = await _binaryCacheService.CacheBuiltPlugin(manifest, buildDirectory, patches,
        installedEngine.Name,
        platforms);
    var buildInfoJson = _jsonService.Serialize(cachedPlugin);
    var buildInfoFile = _fileSystem.FileInfo.New(Path.Join(buildDirectory.FullName, "build-info.json"));
    await buildInfoFile.WriteAllTextAsync(buildInfoJson);
    return cachedPlugin;
  }
}