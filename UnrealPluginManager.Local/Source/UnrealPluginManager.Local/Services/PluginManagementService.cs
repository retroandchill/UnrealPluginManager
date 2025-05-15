using System.IO.Abstractions;
using LanguageExt;
using Retro.ReadOnlyParams.Annotations;
using Retro.SimplePage;
using Semver;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Files.Plugins;
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
public class PluginManagementService(
    [ReadOnly] IFileSystem fileSystem,
    [ReadOnly] IRemoteService remoteService,
    [ReadOnly] IJsonService jsonService,
    [ReadOnly] IStorageService storageService,
    [ReadOnly] IEngineService engineService,
    [ReadOnly] IPluginService pluginService,
    [ReadOnly] IPluginStructureService pluginStructureService,
    [ReadOnly] ISourceDownloadService sourceDownloadService,
    [ReadOnly] IBinaryCacheService binaryCacheService) : IPluginManagementService {
  private const int DefaultPageSize = 100;

  private const string PluginCacheDirectory = "Plugins";

  /// <inheritdoc />
  public async Task<Option<PluginBuildInfo>> FindLocalPlugin(string pluginName, SemVersion versionRange,
                                                             string engineVersion,
                                                             IReadOnlyCollection<string> platforms) {
    return await binaryCacheService.GetCachedPluginBuild(pluginName, versionRange, engineVersion, platforms);
  }

  /// <inheritdoc />
  public Task<OrderedDictionary<string, Fin<List<PluginOverview>>>> GetPlugins(string searchTerm) {
    return remoteService.GetApiAccessors<IPluginsApi>()
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
    var api = remoteService.GetApiAccessor<IPluginsApi>(remote);
    return await searchTerm.ToEnumerable()
        .PageToEndAsync((y, p) => api.GetPluginsAsync(y, p.PageNumber, p.PageSize), DefaultPageSize)
        .ToListAsync();
  }

  /// <inheritdoc />
  public async Task<PluginVersionInfo> FindTargetPlugin(string pluginName, SemVersionRange versionRange,
                                                        string? engineVersion) {
    var currentlyInstalled = await engineService.GetInstalledPluginVersion(pluginName, engineVersion)
        .MapAsync(x => x.SelectManyAsync(v => pluginService.GetPluginVersionInfo(pluginName, v)));

    var resolved = await currentlyInstalled.OrElseAsync(() => LookupPluginVersion(pluginName, versionRange));
    return resolved.OrElseThrow(() =>
                                    new PluginNotFoundException(
                                        $"Unable to resolve plugin {pluginName} with version {versionRange}."));
  }

  private async Task<Option<PluginVersionInfo>> LookupPluginVersion(string pluginName, SemVersionRange versionRange) {
    var cached = await pluginService.GetPluginVersionInfo(pluginName, versionRange);

    return await cached.OrElseAsync(() => LookupPluginVersionOnCloud(pluginName, versionRange));
  }

  private async Task<Option<PluginVersionInfo>> LookupPluginVersionOnCloud(
      string pluginName, SemVersionRange versionRange) {
    var tasks = remoteService.GetApiAccessors<IPluginsApi>()
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
    var currentlyInstalled = await engineService.GetInstalledPlugins(engineVersion)
        .ToDictionaryAsync(x => x.Name, x => x.Version);

    var allDependencies = root.Dependencies
        .Concat(currentlyInstalled.Select(x => new PluginDependency {
            PluginName = x.Key,
            PluginVersion = SemVersionRange.AtLeast(x.Value)
        }))
        .DistinctBy(x => x.PluginName)
        .ToList();

    var dependencyTasks = pluginService.GetPossibleVersions(allDependencies)
        .Map(x => x.SetInstalledPlugins(currentlyInstalled)).ToEnumerable()
        .Concat(remoteService.GetApiAccessors<IPluginsApi>()
                    .Select((x, i) => x.Api.GetCandidateDependenciesAsync(allDependencies)
                                .Map(y => y.SetRemoteIndex(i))))
        .ToList();

    var dependencyManifest = await SafeTasks.WhenEach(dependencyTasks)
        .Where(x => x.IsCompletedSuccessfully)
        .Select(x => x.Result)
        .Collapse();
    return pluginService.GetDependencyList(root, dependencyManifest);
  }

  /// <inheritdoc />
  public async Task<PluginVersionInfo> UploadPlugin(string pluginName, SemVersion version, string? remote) {
    var plugin = await pluginService.ListLatestVersions(pluginName, SemVersionRange.Equals(version))
        .Map(x => x.Count > 0 ? x[0] : null);
    if (plugin is null) {
      throw new PluginNotFoundException($"Unable to find plugin {pluginName} with version {version}.");
    }

    var remoteName = remote ?? remoteService.DefaultRemoteName;
    if (remoteName is null) {
      throw new RemoteNotFoundException("No default remote configured.");
    }

    var manifest = plugin.ToPluginManifest();
    var patches = await pluginService.GetSourcePatches(plugin.PluginId, plugin.VersionId)
        .Map(x => x.Select(y => y.Content)
                 .ToList());
    if (manifest.Patches.Count != patches.Count) {
      throw new BadSubmissionException("Patches are not the same length as the source files.");
    }

    await using var iconFile = plugin.Icon is not null
        ? storageService.GetResourceStream(plugin.Icon.StoredFilename)
        : null;

    var readme = await pluginService.GetPluginReadme(plugin.PluginId, plugin.VersionId)
        .ContinueWith(x => x.IsFaulted ? null : x.Result);

    var pluginsApi = remoteService.GetApiAccessor<IPluginsApi>(remoteName);
    using var memoryStream = new MemoryStream();
    await pluginStructureService.CompressPluginSubmission(new PluginSubmission(manifest, patches, iconFile, readme),
                                                          memoryStream);
    memoryStream.Position = 0;

    return await pluginsApi.SubmitPluginAsync(new FileParameter(memoryStream));
  }

  /// <inheritdoc />
  public async Task<PluginBuildInfo> DownloadPlugin(string pluginName, SemVersion version,
                                                    int? remote, string engineVersion,
                                                    List<string> platforms) {
    var plugin = await binaryCacheService.GetCachedPluginBuild(pluginName, version, engineVersion, platforms);
    return await plugin.OrElseGetAsync(async () => {
      if (!remote.HasValue) {
        throw new PluginNotFoundException(
            $"Unable to find plugin {pluginName} with version {version} in the local cache.");
      }

      var (remoteName, _) = remoteService.GetAllRemotes().GetAt(remote.Value);
      var pluginsApi = remoteService.GetApiAccessor<IPluginsApi>(remoteName);
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
    var installedEngine = engineService.GetInstalledEngine(engineVersion);

    var pluginDirectoryName = Path.Join(storageService.BaseDirectory,
                                        PluginCacheDirectory, manifest.Name, manifest.Version.ToString());
    var pluginDirectory = fileSystem.DirectoryInfo.New(pluginDirectoryName);
    pluginDirectory.Create();

    var sourceDirectoryName = Path.Join(pluginDirectoryName, "Source");
    var sourceDirectory = fileSystem.DirectoryInfo.New(sourceDirectoryName);
    sourceDirectory.Create();

    var upluginFile = sourceDirectory
        .EnumerateFiles("*.uplugin", SearchOption.AllDirectories)
        .FirstOrDefault();
    if (upluginFile is null) {
      await sourceDownloadService.DownloadAndExtractSources(manifest.Source, sourceDirectory);

      upluginFile = sourceDirectory
          .EnumerateFiles("*.uplugin", SearchOption.AllDirectories)
          .FirstOrDefault();
      if (upluginFile is null) {
        throw new ContentNotFoundException("Missing a .uplugin file in the plugin's source directory.");
      }

      var rootDir = upluginFile.Directory.RequireNonNull();
      await sourceDownloadService.PatchSources(rootDir, patches);
    }


    var buildDirectoryName = Path.Join(pluginDirectory.FullName, "Builds", Guid.CreateVersion7().ToString());
    var buildDirectory = fileSystem.DirectoryInfo.New(buildDirectoryName);
    buildDirectory.Create();
    if (await engineService.BuildPlugin(upluginFile, buildDirectory, engineVersion, platforms) != 0) {
      throw new InvalidOperationException();
    }

    var cachedPlugin = await binaryCacheService.CacheBuiltPlugin(manifest, buildDirectory, patches,
                                                                 installedEngine.Name,
                                                                 platforms);
    var buildInfoJson = jsonService.Serialize(cachedPlugin);
    var buildInfoFile = fileSystem.FileInfo.New(Path.Join(buildDirectory.FullName, "build-info.json"));
    await buildInfoFile.WriteAllTextAsync(buildInfoJson);
    return cachedPlugin;
  }
}