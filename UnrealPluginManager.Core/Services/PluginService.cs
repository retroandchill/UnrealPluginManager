using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Model.Plugins;

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
}