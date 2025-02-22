using LanguageExt;
using UnrealPluginManager.Cli.Factories;
using UnrealPluginManager.Core.Mappers;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Pagination;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.WebClient.Api;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Cli.Services;

/// <summary>
/// Service responsible for handling remote API calls related to plugin management.
/// </summary>
public class RemoteCallService : IRemoteCallService {
    private readonly Lazy<Task<OrderedDictionary<string, IPluginsApi>>> _pluginsApis;
    
    private const int DefaultPageSize = 100;

    /// <summary>
    /// Service responsible for managing interactions with remote APIs related to plugins.
    /// This service handles plugin data retrieval by aggregating results from multiple APIs.
    /// </summary>
    public RemoteCallService(IApiAccessorFactory<IPluginsApi> pluginsApiFactory) {
        _pluginsApis = new Lazy<Task<OrderedDictionary<string, IPluginsApi>>>(pluginsApiFactory.GetAccessors);
    }

    /// <inheritdoc />
    public async Task<OrderedDictionary<string, Fin<List<PluginOverview>>>> GetPlugins(string searchTerm) {
        return (await _pluginsApis.Value).ToOrderedDictionary(x => {
            try {
                return searchTerm.AsEnumerable()
                    .PageToEnd((y, p) => x.GetPlugins(searchTerm, p.PageNumber, p.PageSize), DefaultPageSize)
                    .ToList();
            } catch (ApiException e) {
                return Fin<List<PluginOverview>>.Fail(e);
            }
        });
    }

    /// <inheritdoc />
    public async Task<List<PluginOverview>> GetPlugins(string remote, string searchTerm) {
        var apis = await _pluginsApis.Value;
        if (!apis.TryGetValue(remote, out var api)) {
            throw new ArgumentException($"Remote not found '{remote}'");
        }
        
        return searchTerm.AsEnumerable()
            .PageToEnd((y, p) => api.GetPlugins(searchTerm, p.PageNumber, p.PageSize), DefaultPageSize)
            .ToList();
    }
}