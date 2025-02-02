using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Engine;
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
            .Select(p => new PluginSummary(p.Name, p.Description))
            .ToList();
    }

    /// <inheritdoc/>
    public IEnumerable<PluginSummary> GetDependencyList(string pluginName) {
        return dbContext.Plugins.FromSqlRaw(
                """
                WITH PluginData (Id, Name, Description) as (
                    SELECT Id, Name, Description
                    FROM Plugins
                    WHERE Plugins.Name = '{0}'
                    UNION ALL
                    SELECT e.Id, e.Name, e.Description
                    FROM Plugins as e
                    INNER JOIN Dependency d on e.Id = d.ChildId
                    INNER JOIN PluginData o
                        ON d.ParentId = o.Id
                )
                SELECT DISTINCT * FROM PluginData;
                """, pluginName)
            .AsNoTrackingWithIdentityResolution()
            .Select(p => new PluginSummary(p.Name, p.Description))
            .ToList();
    }

    /// <inheritdoc/>
    public PluginSummary AddPlugin(string pluginName, PluginDescriptor descriptor) {
        var plugin = MapBasePluginInfo(pluginName, descriptor);

        var pluginNames = descriptor.Plugins
            .Select(x => x.Name)
            .ToHashSet();
        var foundPlugins = dbContext.Plugins
            .Where(x => pluginNames.Contains(x.Name))
            .ToDictionary(x => x.Name.ToLower(), x => x.Id);
        plugin.DependsOn = pluginNames
            .Select(x => foundPlugins.ResolvePluginName(x))
            .Select(x => new Dependency {
                ChildId = x
            })
            .ToList();
        

        dbContext.Plugins.Add(plugin);
        dbContext.SaveChanges();
        return new PluginSummary(plugin.Name, plugin.Description);
    }

    public IEnumerable<PluginSummary> ImportPlugins(string pluginsFolder, Version? engineVersion = null) {
        var plugins = Directory.EnumerateFiles(pluginsFolder, "*.uplugin", SearchOption.AllDirectories)
            .Select(x => (x, PluginType.Provided));
        return ImportPluginFiles(plugins, engineVersion, true);
    }

    /// <inheritdoc/>
    public IEnumerable<PluginSummary> ImportEnginePlugins(string pluginsFolder, Version? engineVersion = null) {
        var plugins = Directory.EnumerateFiles(pluginsFolder, "*.uplugin", SearchOption.AllDirectories)
            .Select(x => (x, x.StartsWith(Path.Join(pluginsFolder, "Marketplace")) ? PluginType.External : PluginType.Engine));
        return ImportPluginFiles(plugins, engineVersion);
    }

    private IEnumerable<PluginSummary> ImportPluginFiles(IEnumerable<(string, PluginType)> plugins, Version? engineVersion = null, bool queryForMissing = false) {
        var pluginDescriptors = plugins
            .Select(filePath => PluginUtils.ReadPluginDescriptorFromFile(filePath.Item1).Add(filePath.Item2))
            .ToList();
        
        using var transaction = dbContext.Database.BeginTransaction();
        var pluginEntities = pluginDescriptors
            .ToDictionary(x => x.Item1.ToLower(), x => MapBasePluginInfo(x.Item1, x.Item2, x.Item3, engineVersion));
        dbContext.Plugins.AddRange(pluginEntities.Values);

        if (queryForMissing) {
            var pluginNames = pluginDescriptors
                .SelectMany(x => x.Item2.Plugins)
                .Select(x => x.Name)
                .Where(x => !pluginEntities.ContainsKey(x.ToLower()))
                .ToHashSet();
            foreach (var plugin in dbContext.Plugins.Where(x => pluginNames.Contains(x.Name))) {
                pluginEntities[plugin.Name.ToLower()] = plugin;
            }
        }

        foreach (var plugin in pluginDescriptors) {
            var entity = pluginEntities[plugin.Item1.ToLower()];
            entity.DependsOn = plugin.Item2.Plugins
                .Select(x => new Dependency {
                    Child = pluginEntities.ResolvePluginName(x.Name), 
                    Optional = x.Optional
                })
                .ToList();
         
        }
        
        dbContext.SaveChanges();
        transaction.Commit();
        return pluginEntities.Values
            .Select(x => new PluginSummary(x.Name, x.Description))
            .ToList();
    }

    private static Plugin MapBasePluginInfo(string pluginName, PluginDescriptor descriptor, PluginType pluginType = PluginType.Provided, Version? engineVersion = null) {
        var plugin = new Plugin {
            Name = pluginName,
            FriendlyName = descriptor.FriendlyName,
            Description = descriptor.Description,
            CompatibleEngineVersions = new List<EngineVersion>(),
            Type = pluginType,
        };

        if (descriptor.EngineVersion is not null) {
            plugin.CompatibleEngineVersions.Add(new EngineVersion(descriptor.EngineVersion));
        }

        if (engineVersion is not null &&
            !plugin.CompatibleEngineVersions.Any(x =>
                x.Major == engineVersion.Major && x.Minor == engineVersion.Minor)) {
            plugin.CompatibleEngineVersions.Add(new EngineVersion(engineVersion));
        }

        if (!string.IsNullOrWhiteSpace(descriptor.CreatedBy)) {
            plugin.Authors.Add(new PluginAuthor {
                AuthorName = descriptor.CreatedBy,
                AuthorWebsite = descriptor.CreatedByURL
            });
        }

        return plugin;
    }
}