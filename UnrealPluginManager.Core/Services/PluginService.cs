using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Engine;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Services;

public class PluginService(UnrealPluginManagerContext dbContext) : IPluginService {
    public IEnumerable<PluginSummary> GetPluginSummaries() {
        return dbContext.Plugins
            .Select(p => new PluginSummary(p.Name, p.Description))
            .ToList();
    }

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

    public PluginSummary AddPlugin(string pluginName, PluginDescriptor descriptor) {
        var plugin = new Plugin {
            Name = pluginName,
            FriendlyName = descriptor.FriendlyName,
            Description = descriptor.Description,
            CompatibleEngineVersions = new List<EngineVersion>()
        };

        if (descriptor.EngineVersion is not null) {
            plugin.CompatibleEngineVersions.Add(new EngineVersion(descriptor.EngineVersion));
        }

        var pluginNames = descriptor.Plugins
            .Select(x => x.Name)
            .ToHashSet();
        var foundPlugins = dbContext.Plugins
            .Where(x => pluginNames.Contains(x.Name))
            .ToDictionary(x => x.Name, x => x.Id);
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
}