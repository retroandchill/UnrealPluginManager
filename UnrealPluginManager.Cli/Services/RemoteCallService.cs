using System.CommandLine;
using LanguageExt;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Pagination;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.WebClient.Api;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Cli.Services;

/// <summary>
/// Service responsible for handling remote API calls related to plugin management.
/// </summary>
[AutoConstructor]
public partial class RemoteCallService : IRemoteCallService {
    private readonly IConsole _console;
    private readonly IRemoteService _remoteService;
    
    private const int DefaultPageSize = 100;

    /// <inheritdoc />
    public Task<OrderedDictionary<string, Fin<List<PluginOverview>>>> GetPlugins(string searchTerm) {
        return _remoteService.GetApiAccessors<IPluginsApi>()
            .ToOrderedDictionaryAsync(async x => {
                try {
                    return await searchTerm.AsEnumerable()
                        .PageToEndAsync((y, p) => x.GetPluginsAsync(y, p.PageNumber, p.PageSize), DefaultPageSize)
                        .ToListAsync();
                } catch (ApiException e) {
                    return Fin<List<PluginOverview>>.Fail(e);
                }
            });
    }

    /// <inheritdoc />
    public async Task<List<PluginOverview>> GetPlugins(string remote, string searchTerm) {
        var api = _remoteService.GetApiAccessor<IPluginsApi>(remote);
        return await searchTerm.AsEnumerable()
            .PageToEndAsync((y, p) => api.GetPluginsAsync(y, p.PageNumber, p.PageSize), DefaultPageSize)
            .ToListAsync();
    }

    /// <inheritdoc />
    public async Task<DependencyManifest> TryResolveRemoteDependencies(List<PluginDependency> rootDependencies,
        DependencyManifest localManifest) {
        if (localManifest.UnresolvedDependencies.Count == 0) {
            return localManifest;
        }
        
        var pluginApis = _remoteService.GetApiAccessors<IPluginsApi>();
        foreach (var (name, api) in pluginApis) {
            var allDependencies = rootDependencies
                .Concat(localManifest.FoundDependencies.Values
                    .SelectMany(x => x)
                    .SelectMany(x => x.Dependencies))
                .DistinctBy(x => (x.PluginName, x.PluginVersion))
                .ToList();
            DependencyManifest supplementalDependencies;
            try {
                supplementalDependencies = await api.GetCandidateDependenciesAsync(allDependencies);
            } catch (ApiException e) {
                _console.WriteLine($"Warning: {e.Message}");
                continue;
            }

            foreach (var plugin in supplementalDependencies.FoundDependencies.Values.SelectMany(x => x)) {
                plugin.RemoteName = name;
            }
            
            localManifest = localManifest.Merge(supplementalDependencies);

            if (localManifest.UnresolvedDependencies.Count == 0) {
                return localManifest;
            }
        }

        throw new DependencyResolutionException(
            $"Unable to resolve dependencies:\n{string.Join("\n", localManifest.UnresolvedDependencies)}");
    }
}