using Microsoft.AspNetCore.Mvc;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class PluginsController(IPluginService pluginService) : ControllerBase {
    [HttpGet(Name = "GetAllPlugins")]
    public IEnumerable<PluginSummary> Get() {
        return pluginService.GetPluginSummaries();
    }

    [HttpPost(Name = "AddPlugin")]
    public PluginDescriptor Post(PluginDescriptor pluginDescriptor) {
        throw new NotImplementedException();
    }
    
}