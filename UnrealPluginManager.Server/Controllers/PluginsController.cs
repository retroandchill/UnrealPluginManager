using Microsoft.AspNetCore.Mvc;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;

namespace UnrealPluginManager.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class PluginsController(UnrealPluginManagerContext context) : ControllerBase {
    
    private readonly UnrealPluginManagerContext _context = context;

    [HttpGet(Name = "GetAllPlugins")]
    public IEnumerable<Plugin> Get()
    {
        return _context.Plugins.ToList();
    }
    
}