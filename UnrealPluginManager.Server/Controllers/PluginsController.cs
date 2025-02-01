using Microsoft.AspNetCore.Mvc;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Filters;

namespace UnrealPluginManager.Server.Controllers;

[ApiController]
[Route("[controller]")]
[ApiExceptionFilter]
public class PluginsController(IPluginService pluginService) : ControllerBase {
    [HttpGet(Name = "GetAllPlugins")]
    public IEnumerable<PluginSummary> Get() {
        return pluginService.GetPluginSummaries();
    }

    [HttpPost(Name = "AddPlugin")]
    public PluginSummary Post([FromQuery] string name, [FromBody] PluginDescriptor descriptor) {
        return pluginService.AddPlugin(name, descriptor);
    }
    
}