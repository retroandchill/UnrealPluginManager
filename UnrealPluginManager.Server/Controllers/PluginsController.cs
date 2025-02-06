using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Http.HttpResults;
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
    [Consumes(MediaTypeNames.Multipart.FormData)]
    public async Task<PluginSummary> Post(IFormFile pluginFile, [FromQuery] Version engineVersion) {
        await using var stream = pluginFile.OpenReadStream();
        return await pluginService.SubmitPlugin(stream, engineVersion);
    }
    
    [HttpGet("{pluginName}")]
    public async Task<List<PluginSummary>> GetDependencyTree([FromRoute] string pluginName) {
        return await pluginService.GetDependencyList(pluginName);
    }
    
    [HttpGet("{pluginName}/download")]
    [Produces(MediaTypeNames.Application.Zip)]
    [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
    public async Task<FileStreamResult> DownloadPlugin([FromRoute] string pluginName, [FromQuery] Version engineVersion) {
        return File(await pluginService.GetPluginFileData(pluginName, engineVersion), MediaTypeNames.Application.Zip, 
            $"{pluginName}.zip");
    }
    
    
    
}