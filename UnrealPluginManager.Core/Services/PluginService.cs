using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Services;

public class PluginService(UnrealPluginManagerContext dbContext) : IPluginService {
    public IEnumerable<PluginSummary> GetPluginSummaries() {
        return dbContext.Plugins
            .Select(p => new PluginSummary(p.Name, p.Description))
            .ToList();
    }
}