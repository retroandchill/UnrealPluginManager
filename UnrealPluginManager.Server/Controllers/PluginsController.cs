using Microsoft.AspNetCore.Mvc;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Filters;

namespace UnrealPluginManager.Server.Controllers;

[ApiController]
[ApiExceptionFilter]
[Route("/api/plugins")]
public class PluginsController(IPluginService pluginService) : ControllerBase {
    [HttpGet]
    public IEnumerable<PluginSummary> Get() {
        return pluginService.GetPluginSummaries();
    }

    [HttpPost]
    public PluginSummary Post([FromQuery] string name, [FromBody] PluginDescriptor descriptor) {
        return pluginService.AddPlugin(name, descriptor);
    }
    
    [HttpGet("/{pluginName}")]
    public IEnumerable<PluginSummary> GetDependencyTree([FromRoute] string pluginName) {
        return pluginService.GetDependencyList(pluginName);
    }
    
    
    
}