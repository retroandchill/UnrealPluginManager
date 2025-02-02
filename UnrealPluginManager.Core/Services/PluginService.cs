using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Model.Plugins;
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
            .Single();

        var unresolved = new HashSet<string>();
        var found = new Dictionary<string, Plugin>();
        unresolved.UnionWith(plugin.Dependencies
            .Where(x => x.Type == PluginType.Provided)
            .Select(x => x.PluginName));
        while (unresolved.Any()) {
            var plugins = dbContext.Plugins
                .Include(p => p.Dependencies)
                .Where(p => unresolved.Contains(p.Name))
                .ToList();
            
            var candidates = plugins
                .GroupBy(p => p.Name)
                .Select(x => x.OrderByDescending(y => y.Version).First());

            foreach (var candidate in candidates) {
                if (found.ContainsKey(candidate.Name)) {
                    continue;
                }
                
                found.Add(candidate.Name, candidate);
                unresolved.Remove(candidate.Name);
                unresolved.UnionWith(candidate.Dependencies
                    .Where(x => !found.ContainsKey(x.PluginName) && x.Type == PluginType.Provided)
                    .Select(x => x.PluginName));
            }
        }
        
        return (new [] { plugin }).Union(found.Values)
            .Select(x => new PluginSummary(x.Name, x.Version, x.Description))
            .ToList();
    }

    /// <inheritdoc/>
    public PluginSummary AddPlugin(string pluginName, PluginDescriptor descriptor) {
        var plugin = MapBasePluginInfo(pluginName, descriptor);
        
        plugin.Dependencies = descriptor.Plugins
            .Select(x => new Dependency {
                PluginName = x.Name,
                Optional = x.Optional,
                Type = x.PluginType
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

    private IEnumerable<PluginSummary> ImportPluginFiles(IEnumerable<(string, PluginType)> plugins) {
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
            AuthorWebsite = descriptor.CreatedByURL,
            Dependencies = descriptor.Plugins
                .Select(x => new Dependency {
                    PluginName = x.Name,
                    Type = x.PluginType
                })
                .ToList()
        };

        return plugin;
    }
}