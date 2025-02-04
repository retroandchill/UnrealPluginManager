using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Solver;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Provides operations for managing plugins within the Unreal Plugin Manager application.
/// </summary>
public class PluginService(UnrealPluginManagerContext dbContext) : IPluginService {
    /// <inheritdoc/>
    public IEnumerable<PluginSummary> GetPluginSummaries() {
        return dbContext.Plugins
            .Select(p => new PluginSummary(p.Name, p.Version, p.Description))
            .ToList();
    }

    /// <inheritdoc/>
    public IEnumerable<PluginSummary> GetDependencyList(string pluginName) {
        var plugin = dbContext.Plugins
            .Include(p => p.Dependencies)
            .Where(p => p.Name == pluginName)
            .OrderByDescending(p => p.Version)
            .First();

        var pluginData = new Dictionary<string, List<Plugin>> { { plugin.Name, [plugin] } };
        var unresolved = new System.Collections.Generic.HashSet<string>();

        unresolved.UnionWith(plugin.Dependencies
            .Where(x => x.Type == PluginType.Provided)
            .Select(pd => pd.PluginName));
        while (unresolved.Count > 0) {
            var currentlyExisting = unresolved.ToHashSet();
            var plugins = dbContext.Plugins
                .Include(p => p.Dependencies)
                .Where(p => unresolved.Contains(p.Name))
                .OrderByDescending(p => p.Version)
                .AsEnumerable()
                .GroupBy(x => x.Name);

            foreach (var pluginList in plugins) {
                unresolved.Remove(pluginList.Key);
                var asList = pluginList
                    .OrderByDescending(p => p.Version)
                    .ToList();
                pluginData.Add(pluginList.Key, asList);

                unresolved.UnionWith(pluginList
                    .SelectMany(p => p.Dependencies)
                    .Where(x => x.Type == PluginType.Provided && !pluginData.ContainsKey(x.PluginName))
                    .Select(pd => pd.PluginName));
            }

            var intersection = currentlyExisting.Intersect(unresolved).ToList();
            if (intersection.Count > 0) {
                throw new DependencyResolutionException($"Unable to resolve plugin names:\n{string.Join("\n", intersection)}");
            }
        }

        var formula = ExpressionSolver.Convert(pluginName, plugin.Version, pluginData);
        var bindings = formula.Solve();
        if (bindings.IsNone) {
            return [];
        }
        
        return bindings.SelectMany(b => b)
            .Select(p => pluginData[p.Item1].First(d => d.Version == p.Item2))
            .Select(p => new PluginSummary(p.Name, p.Version, p.Description))
            .ToList();
    }

    /// <inheritdoc/>
    public PluginSummary AddPlugin(string pluginName, PluginDescriptor descriptor) {
        var plugin = MapBasePluginInfo(pluginName, descriptor);

        plugin.Dependencies = descriptor.Plugins
            .Select(x => new Dependency {
                PluginName = x.Name,
                Optional = x.Optional,
                Type = x.PluginType,
                PluginVersion = x.VersionMatcher
            })
            .ToList();


        dbContext.Plugins.Add(plugin);
        dbContext.SaveChanges();
        return new PluginSummary(plugin.Name, plugin.Version, plugin.Description);
    }

    public IEnumerable<PluginSummary> ImportPlugins(string pluginsFolder) {
        var plugins = Directory.EnumerateFiles(pluginsFolder, "*.uplugin", SearchOption.AllDirectories)
            .Select(x => (x, PluginType.Provided));
        return ImportPluginFiles(plugins);
    }

    private List<PluginSummary> ImportPluginFiles(IEnumerable<(string, PluginType)> plugins) {
        var pluginDescriptors = plugins
            .Select(filePath => PluginUtils.ReadPluginDescriptorFromFile(filePath.Item1).Add(filePath.Item2))
            .ToList();

        using var transaction = dbContext.Database.BeginTransaction();
        var pluginEntities = pluginDescriptors
            .ToDictionary(x => x.Item1.ToLower(), x => MapBasePluginInfo(x.Item1, x.Item2));
        dbContext.Plugins.AddRange(pluginEntities.Values);

        dbContext.SaveChanges();
        transaction.Commit();
        return pluginEntities.Values
            .Select(x => new PluginSummary(x.Name, x.Version, x.Description))
            .ToList();
    }

    private static Plugin MapBasePluginInfo(string pluginName, PluginDescriptor descriptor) {
        var plugin = new Plugin {
            Name = pluginName,
            FriendlyName = descriptor.FriendlyName,
            Version = descriptor.VersionName,
            Description = descriptor.Description,
            AuthorName = !string.IsNullOrWhiteSpace(descriptor.CreatedBy) ? descriptor.CreatedBy : null,
            AuthorWebsite = descriptor.CreatedByUrl,
            Dependencies = descriptor.Plugins
                .Select(x => new Dependency {
                    PluginName = x.Name,
                    Type = x.PluginType,
                    PluginVersion = x.VersionMatcher
                })
                .ToList()
        };

        return plugin;
    }
}