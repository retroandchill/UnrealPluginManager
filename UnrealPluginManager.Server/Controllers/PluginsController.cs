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
    public async Task<List<PluginSummary>> Get() {
        return await pluginService.GetPluginSummaries();
    }

    [HttpPost]
    public async Task<PluginSummary> Post([FromQuery] string name, [FromBody] PluginDescriptor descriptor) {
        return await pluginService.AddPlugin(name, descriptor);
    }
    
    [HttpGet("/{pluginName}")]
    public async Task<List<PluginSummary>> GetDependencyTree([FromRoute] string pluginName) {
        return await pluginService.GetDependencyList(pluginName);
    }
    
    
    
}