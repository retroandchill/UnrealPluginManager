using System.Net.Mime;
using LanguageExt;
using Semver;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Resolution;
using UnrealPluginManager.Core.Pagination;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Local.Exceptions;
using UnrealPluginManager.WebClient.Api;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// Service responsible for handling remote API calls related to plugin management.
/// </summary>
[AutoConstructor]
public partial class PluginManagementService : IPluginManagementService {
  private readonly IRemoteService _remoteService;
  private readonly IEngineService _engineService;
  private readonly IPluginService _pluginService;

  private const int DefaultPageSize = 100;

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
  public async Task<PluginVersionInfo> FindTargetPlugin(string pluginName, SemVersionRange versionRange, string? engineVersion) {
    var currentlyInstalled = await _engineService.GetInstalledPluginVersion(pluginName, engineVersion)
        .MapAsync(x => x.SelectManyAsync(v => _pluginService.GetPluginVersionInfo(pluginName, v)));

    var resolved = await currentlyInstalled.OrElseAsync(() => LookupPluginVersion(pluginName, versionRange));
    return resolved.OrElseThrow(() => new PluginNotFoundException($"Unable to resolve plugin {pluginName} with version {versionRange}."));
  }

  private async Task<Option<PluginVersionInfo>> LookupPluginVersion(string pluginName, SemVersionRange versionRange) {
    var cached = await _pluginService.GetPluginVersionInfo(pluginName, versionRange);
    
    return await cached.OrElseAsync(() => LookupPluginVersionOnCloud(pluginName, versionRange));
  }

  private async Task<Option<PluginVersionInfo>> LookupPluginVersionOnCloud(string pluginName, SemVersionRange versionRange) {
    var tasks = _remoteService.GetApiAccessors<IPluginsApi>()
        .Select(x => pluginName.ToEnumerable()
                    .PageToEndAsync((y, p) => x.Api.GetLatestVersionsAsync(y, versionRange.ToString(), p.PageNumber, p.PageSize))
                    .FirstOrDefaultAsync()
                    .ToRef())
        .ToList();
    
    await Task.WhenAll(tasks);
    return tasks.Where(version => !version.IsFaulted)
        .Select(x => x.Result)
        .FirstOrDefault(version => version is not null)
        .ToOption();
  }

  /// <inheritdoc />
  public async Task<List<PluginSummary>> GetPluginsToInstall(IDependencyChainNode root, string? engineVersion) {
    var currentlyInstalled = await _engineService.GetInstalledPlugins(engineVersion)
        .ToDictionaryAsync(x => x.Name, x => x.Version);
    
    var allDependencies = root.Dependencies
        .Concat(currentlyInstalled.Select(x => new PluginDependency {
            PluginName = x.Key,
            PluginVersion = SemVersionRange.AtLeast(x.Value),
            Type = PluginType.Provided
        }))
        .DistinctBy(x => x.PluginName)
        .ToList();
    
    var dependencyTasks = _pluginService.GetPossibleVersions(allDependencies)
        .Map(x => x.SetInstalledPlugins(currentlyInstalled)).ToEnumerable()
        .Concat(_remoteService.GetApiAccessors<IPluginsApi>()
                    .Select((x, i) => x.Api.GetCandidateDependenciesAsync(allDependencies)
                                .Map(y => y.SetRemoteIndex(i))))
        .ToList();

    var dependencyManifest = await Task.WhenEach(dependencyTasks)
        .Where(x => x.IsCompletedSuccessfully)
        .Select(x => x.Result)
        .Collapse();
    return _pluginService.GetDependencyList(root, dependencyManifest);
  }

  /// <inheritdoc />
  public async Task<PluginDetails> UploadPlugin(string pluginName, SemVersion version, string? remote) {
    var plugin = await _pluginService.ListLatestedVersions(pluginName, SemVersionRange.Equals(version))
        .Map(x => x.Count > 0 ? x[0] : null);
    if (plugin is null) {
      throw new PluginNotFoundException($"Unable to find plugin {pluginName} with version {version}.");
    }

    var remoteName = remote ?? _remoteService.DefaultRemoteName;
    if (remoteName is null) {
      throw new RemoteNotFoundException("No default remote configured.");
    }
    
    await using var fileData = await _pluginService.GetPluginFileData(plugin.PluginId, plugin.VersionId)
        .Map(x => x.FileData);
    var pluginsApi = _remoteService.GetApiAccessor<IPluginsApi>(remoteName);
    return await pluginsApi.SubmitPluginAsync(new FileParameter("submission", fileData));
  }
}