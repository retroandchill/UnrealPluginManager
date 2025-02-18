﻿using System.Net;
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
[AutoConstructor]
public partial class PluginsController : ControllerBase {
    private readonly IPluginService _pluginService;

    /// <summary>
    /// Retrieves a paginated list of plugin overviews based on the specified filter and pagination settings.
    /// </summary>
    /// <param name="match">A wildcard string used to filtered plugins by name. Defaults to "*".</param>
    /// <param name="pageable">Pagination settings including page size, index, and sort order.</param>
    /// <return>Returns a paginated collection of plugin overviews.</return>
    [HttpGet]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(Page<PluginOverview>), (int) HttpStatusCode.OK)]
    public async Task<Page<PluginOverview>> GetPlugins([FromQuery] string match = "*", [FromQuery] Pageable pageable = default) {
        return await _pluginService.ListPlugins(match, pageable);
    }

    /// <summary>
    /// Adds a plugin by uploading a plugin file and specifying the target Unreal Engine version.
    /// </summary>
    /// <param name="pluginFile">The uploaded plugin file in a valid format.</param>
    /// <param name="engineVersion">The target Unreal Engine version for which the plugin is being added.</param>
    /// <return>Returns detailed information about the added plugin.</return>
    [HttpPost]
    [Consumes(MediaTypeNames.Multipart.FormData)]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(PluginDetails), (int) HttpStatusCode.OK)]
    public async Task<PluginDetails> AddPlugin(IFormFile pluginFile, [FromQuery] Version engineVersion) {
        await using var stream = pluginFile.OpenReadStream();
        return await _pluginService.SubmitPlugin(stream, engineVersion);
    }


    /// <summary>
    /// Retrieves the dependency tree for a specified plugin.
    /// </summary>
    /// <param name="pluginName">The name of the plugin whose dependency tree is to be retrieved.</param>
    /// <return>Returns a list of plugin summaries representing the dependency tree.</return>
    [HttpGet("{pluginName}")]
    [Produces(MediaTypeNames.Application.Json)]
    [ProducesResponseType(typeof(List<PluginSummary>), (int)HttpStatusCode.OK)]
    public async Task<List<PluginSummary>> GetDependencyTree([FromRoute] string pluginName) {
        return await _pluginService.GetDependencyList(pluginName);
    }


    /// <summary>
    /// Downloads a plugin file as a ZIP archive for the specified plugin and engine version.
    /// </summary>
    /// <param name="pluginName">The name of the plugin to be downloaded.</param>
    /// <param name="engineVersion">The Unreal Engine version for which the plugin file is requested.</param>
    /// <return>Returns a FileStreamResult containing the plugin file as a ZIP archive.</return>
    [HttpGet("{pluginName}/download")]
    [Produces(MediaTypeNames.Application.Zip)]
    [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
    public async Task<FileStreamResult> DownloadPlugin([FromRoute] string pluginName, [FromQuery] Version engineVersion) {
        return File(await _pluginService.GetPluginFileData(pluginName, SemVersionRange.All, engineVersion.ToString()), MediaTypeNames.Application.Zip,
            $"{pluginName}.zip");
    }
}