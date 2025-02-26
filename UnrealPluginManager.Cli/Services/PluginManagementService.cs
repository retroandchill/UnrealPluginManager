using System.CommandLine;
using LanguageExt;
using Semver;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Mappers;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Pagination;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.WebClient.Api;
using UnrealPluginManager.WebClient.Client;
using StringSet = System.Collections.Generic.HashSet<string>;

namespace UnrealPluginManager.Cli.Services;

/// <summary>
/// Service responsible for handling remote API calls related to plugin management.
/// </summary>
[AutoConstructor]
public partial class PluginManagementService : IPluginManagementService {
    private readonly IConsole _console;
    private readonly IRemoteService _remoteService;
    private readonly IEngineService _engineService;
    private readonly IPluginService _pluginService;
    
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

    private async IAsyncEnumerable<DependencyManifest> TryResolveRemoteDependencies(List<PluginDependency> pluginsList) {
        var resolvedDependencies = await _pluginService.GetPossibleVersions(pluginsList);
        yield return resolvedDependencies;
        
        var pluginApis = _remoteService.GetApiAccessors<IPluginsApi>();
        foreach (var (name, api) in pluginApis) {
            var allDependencies = pluginsList
                .Concat(resolvedDependencies.FoundDependencies.Values
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
            
            resolvedDependencies = resolvedDependencies.Merge(supplementalDependencies);
            yield return resolvedDependencies;
        }
    }

    /// <inheritdoc />
    public async Task<List<PluginSummary>> ResolveDependenciesForFile(string filename, string? engineVersion) {
        var pluginData = await _engineService.ReadSubmittedPluginFile(filename);
        var pluginsList = pluginData.Plugins.Select(x => x.ToPluginDependency()).ToList();

        var currentlyInstalled = (await _engineService.GetInstalledPluginVersions(engineVersion))
            .ToDictionary(x => x.Name);

        var dependencies = new LanguageExt.Option<List<PluginSummary>>();
        var unsolvedDependencies = new StringSet();
        await foreach (var manifest in TryResolveRemoteDependencies(pluginsList)) {
            foreach (var name in manifest.FoundDependencies.Keys) {
                if (currentlyInstalled.TryGetValue(name, out var installed)) {
                    manifest.FoundDependencies[name] = installed.AsEnumerable()
                        .Concat(manifest.FoundDependencies[name])
                        .DistinctBy(x => (x.Name, x.Version))
                        .OrderByDescending(x => x.IsInstalled)
                        .ThenByDescending(x => x.Version, SemVersion.PrecedenceComparer)
                        .ToList();
                } else {
                    manifest.FoundDependencies[name] = manifest.FoundDependencies[name]
                        .OrderByDescending(x => x.Version, SemVersion.PrecedenceComparer)
                        .ToList();
                }
            }

            unsolvedDependencies = manifest.UnresolvedDependencies;
            dependencies = _pluginService.GetDependencyList(pluginsList, manifest);

            if (dependencies.IsSome) {
                break;
            }
        }

        return dependencies.OrElseThrow(() => new DependencyResolutionException(
            $"Unable to resolve dependencies:\n{string.Join("\n", unsolvedDependencies)}"));
    }

    public async Task InstallPlugins(IEnumerable<PluginSummary> plugins) {
        foreach (var plugin in plugins) {
            if (await _pluginService.IsPluginCached(plugin.PluginId, plugin.VersionId)) {
                
            }
        }
    }
}