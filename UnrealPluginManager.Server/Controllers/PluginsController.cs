using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Semver;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Pagination;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Filters;

namespace UnrealPluginManager.Server.Controllers;

/// <summary>
/// The PluginsController class provides HTTP API endpoints to manage plugins within the Unreal Plugin Manager system.
/// </summary>
/// <remarks>
/// This controller handles operations related to plugins, such as retrieving plugin summaries, uploading new plugins,
/// fetching dependency trees for plugins, and downloading plugins in ZIP format.
/// It relies on the IPluginService interface for interacting with plugin data and processing business logic.
/// </remarks>
/// <example>
/// The controller supports HTTP actions like GET, POST, and retrieving files through endpoints defined via attributes.
/// The controller is decorated with <c>ApiExceptionFilter</c> to handle exceptions in a consistent manner.
/// </example>
/// <seealso cref="UnrealPluginManager.Core.Services.IPluginService"/>
[ApiController]
[ApiExceptionFilter]
[Route("/api/plugins")]
public class PluginsController(IPluginService pluginService) : ControllerBase {
    /// Retrieves the list of plugin summaries from the plugin service.
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of plugin summaries.
    /// </returns>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(Page<PluginOverview>), (int)HttpStatusCode.OK)]
    public async Task<Page<PluginOverview>> GetPlugins([FromQuery] string match = "*", [FromQuery] Pageable pageable = default) {
        return await pluginService.ListPlugins(match, pageable);
    }

    /// Submits a plugin file to the plugin service for processing and management.
    /// <param name="pluginFile">The plugin file to be submitted.</param>
    /// <param name="engineVersion">The version of the engine the plugin is intended for.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the summary of the submitted plugin.
    /// </returns>
    [HttpPost]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(PluginDetails), (int)HttpStatusCode.OK)]
    public async Task<PluginDetails> AddPlugin(IFormFile pluginFile, [FromQuery] Version engineVersion) {
        await using var stream = pluginFile.OpenReadStream();
        return await pluginService.SubmitPlugin(stream, engineVersion);
    }

    /// Retrieves the dependency tree for a specified plugin.
    /// <param name="pluginName">
    /// The name of the plugin for which the dependency tree is to be retrieved.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a list of plugin summaries representing the dependency tree for the specified plugin.
    /// </returns>
    [HttpGet("{pluginName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(List<PluginSummary>), (int)HttpStatusCode.OK)]
    public async Task<List<PluginSummary>> GetDependencyTree([FromRoute] string pluginName) {
        return await pluginService.GetDependencyList(pluginName);
    }

    /// Downloads a plugin as a ZIP file based on the provided plugin name and engine version.
    /// <param name="pluginName">
    /// The name of the plugin to be downloaded.
    /// </param>
    /// <param name="engineVersion">
    /// The version of the Unreal Engine for which the plugin is compatible.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains a file stream result of the plugin ZIP file.
    /// </returns>
    [HttpGet("{pluginName}/download")]
    [Produces(MediaTypeNames.Application.Zip)]
    [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
    public async Task<FileStreamResult> DownloadPlugin([FromRoute] string pluginName, [FromQuery] Version engineVersion) {
        return File(await pluginService.GetPluginFileData(pluginName, SemVersionRange.All, engineVersion.ToString()), MediaTypeNames.Application.Zip,
            $"{pluginName}.zip");
    }
}