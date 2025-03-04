﻿using LanguageExt;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Pagination;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;
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
  public async Task<DependencyManifest> GetPossibleDependencies(IDependencyChainNode root, string? engineVersion) {
    var currentlyInstalled = await _engineService.GetInstalledPlugins(engineVersion)
        .ToDictionaryAsync(x => x.Name, x => x.Version);
    var dependencyTasks = _pluginService.GetPossibleVersions(root.Dependencies)
        .Map(x => x.SetInstalledPlugins(currentlyInstalled)).ToEnumerable()
        .Concat(_remoteService.GetApiAccessors<IPluginsApi>()
                    .Select((x, i) => x.Api.GetCandidateDependenciesAsync(root.Dependencies)
                                .Map(y => y.SetRemoteIndex(i))))
        .ToList();

    return await Task.WhenEach(dependencyTasks)
        .Select(x => x.Result)
        .Collapse();
  }
}