using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mime;
using LanguageExt;
using Microsoft.AspNetCore.Mvc;
using Semver;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Resolution;
using UnrealPluginManager.Core.Pagination;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;

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
  [ProducesResponseType(typeof(Page<PluginOverview>), (int)HttpStatusCode.OK)]
  public Task<Page<PluginOverview>> GetPlugins([FromQuery] string match = "*",
                                               [FromQuery] Pageable pageable = default) {
    return _pluginService.ListPlugins(match, pageable);
  }

  /// <summary>
  /// Adds a plugin by uploading a plugin file and specifying the target Unreal Engine version.
  /// </summary>
  /// <param name="pluginFile">The uploaded plugin file in a valid format.</param>
  /// <param name="engineVersion">The target Unreal Engine version for which the plugin is being added.</param>
  /// <return>Returns detailed information about the added plugin.</return>
  [HttpPost("{engineVersion}/submit")]
  [Consumes(MediaTypeNames.Multipart.FormData)]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(typeof(PluginDetails), (int)HttpStatusCode.OK)]
  public async Task<PluginDetails> AddPlugin(IFormFile pluginFile, [FromRoute] Version engineVersion) {
    await using var stream = pluginFile.OpenReadStream();
    return await _pluginService.SubmitPlugin(stream, engineVersion.ToString());
  }

  /// <summary>
  /// Retrieves detailed information about the latest version of the specified plugin,
  /// optionally constrained by a version range.
  /// </summary>
  /// <param name="pluginName">The name of the plugin to retrieve the latest version for.</param>
  /// <param name="version">An optional version range to filter the plugin's versions. Defaults to all released versions.</param>
  /// <return>Returns details of the latest version of the specified plugin that satisfies the given version range.</return>
  [HttpGet("{pluginName}/latest")]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(typeof(PluginVersionInfo), (int)HttpStatusCode.OK)]
  public async Task<PluginVersionInfo> GetLatestVersion([FromRoute] string pluginName,
                                                        [FromQuery] SemVersionRange? version = null) {
    var latest = await _pluginService.GetPluginVersionInfo(pluginName, version ?? SemVersionRange.AllRelease);
    return latest.OrElseThrow(
        () => new PluginNotFoundException(
            $"Could not find plugin {pluginName} that satisfies the specified version range."));
  }

  /// <summary>
  /// Retrieves the dependency tree for a specified plugin.
  /// </summary>
  /// <param name="pluginName">The name of the plugin whose dependency tree is to be retrieved.</param>
  /// <param name="targetVersion">Optional target version range to filter the dependency tree.</param>
  /// <return>Returns a list of plugin summaries representing the dependency tree.</return>
  [HttpGet("{pluginName}/latest/dependencies")]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(typeof(List<PluginSummary>), (int)HttpStatusCode.OK)]
  public Task<List<PluginSummary>> GetDependencyTree([FromRoute] string pluginName, SemVersionRange? targetVersion = null) {
    return _pluginService.GetDependencyList(pluginName, targetVersion);
  }
  
  /// <summary>
  /// Downloads a plugin file as a ZIP archive for the specified plugin, engine version, and target platforms.
  /// </summary>
  /// <param name="pluginName">The name of the plugin to be downloaded.</param>
  /// <param name="engineVersion">The Unreal Engine version for which the plugin file is requested.</param>
  /// <param name="targetVersion">The semantic version range that specifies the version of the plugin to target. Defaults to all release versions if not specified.</param>
  /// <param name="platforms">The collection of target platforms for which the plugin file is compatible.</param>
  /// <return>Returns a FileStreamResult containing the plugin file as a ZIP archive.</return>
  [HttpGet("{pluginName}/latest/{engineVersion}/download")]
  [Produces(MediaTypeNames.Application.Zip)]
  [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
  public async Task<FileStreamResult> DownloadLatestPlugin([FromRoute] string pluginName,
                                                     [FromRoute] Version engineVersion,
                                                     [FromQuery] SemVersionRange? targetVersion,
                                                     [FromQuery] IReadOnlyCollection<string> platforms) {
    return File(
        await _pluginService.GetPluginFileData(pluginName, targetVersion ?? SemVersionRange.AllRelease, engineVersion.ToString(), platforms),
        MediaTypeNames.Application.Zip,
        $"{pluginName}.zip");
  }


  /// <summary>
  /// Downloads the specified version of a plugin as a ZIP file for the specified Unreal Engine version and target platforms.
  /// </summary>
  /// <param name="pluginName">The name of the plugin to download.</param>
  /// <param name="version">The version of the plugin to download.</param>
  /// <param name="engineVersion">The version of the Unreal Engine for which the plugin is compatible.</param>
  /// <param name="platforms">The collection of target platforms for the plugin.</param>
  /// <return>Returns a ZIP file containing the requested plugin version for the specified engine version and platforms.</return>
  [HttpGet("{pluginName}/{version}/download/{engineVersion}")]
  [Produces(MediaTypeNames.Application.Zip)]
  [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
  public async Task<FileStreamResult> DownloadPluginVersion([FromRoute] string pluginName, [FromRoute] SemVersion version,
                                                     [FromRoute] Version engineVersion,
                                                     [FromQuery] IReadOnlyCollection<string> platforms) {
    return File(
        await _pluginService.GetPluginFileData(pluginName, version, engineVersion.ToString(), platforms),
        MediaTypeNames.Application.Zip,
        $"{pluginName}.zip");
  }

  /// <summary>
  /// Downloads the source code of a specific plugin version as a zip file.
  /// </summary>
  /// <param name="pluginName">The name of the plugin to download.</param>
  /// <param name="version">The specific version of the plugin to download.</param>
  /// <return>Returns a file stream result containing the plugin source code in a zip archive.</return>
  [HttpGet("{pluginName}/{version}/download/source")]
  [Produces(MediaTypeNames.Application.Zip)]
  [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
  public async Task<FileStreamResult> DownloadPluginSource([FromRoute] string pluginName, [FromRoute] SemVersion version) {
    return File(
        await _pluginService.GetPluginSource(pluginName, version).Map(x => x.OpenRead()),
        MediaTypeNames.Application.Zip,
        $"{pluginName}.zip");
  }

  /// <summary>
  /// Downloads the binary files of a specified plugin for a given version, engine version, and platform.
  /// </summary>
  /// <param name="pluginName">The name of the plugin whose binaries are being requested.</param>
  /// <param name="version">The specific version of the plugin to download binaries for.</param>
  /// <param name="engineVersion">The version of the engine for which the plugin binaries are targeted.</param>
  /// <param name="platform">The platform for which the plugin binaries are compiled.</param>
  /// <return>Returns the binary files of the plugin as a file stream result in ZIP format.</return>
  [HttpGet("{pluginName}/{version}/download/{engineVersion}/{platform}/binaries")]
  [Produces(MediaTypeNames.Application.Zip)]
  [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
  public async Task<FileStreamResult> DownloadPluginBinaries([FromRoute] string pluginName, [FromRoute] SemVersion version,
                                                     [FromRoute] Version engineVersion,
                                                     [FromRoute] string platform) {
    return File(
        await _pluginService.GetPluginBinaries(pluginName, version, engineVersion.ToString(), platform).Map(x => x.OpenRead()),
        MediaTypeNames.Application.Zip,
        $"{pluginName}.zip");
  }

  /// <summary>
  /// Retrieves a dependency manifest containing potential versions for the given list of plugin dependencies.
  /// </summary>
  /// <param name="dependencies">A list of plugin dependencies for which potential versions are to be determined.</param>
  /// <return>Returns a dependency manifest with possible versions for the specified dependencies.</return>
  [HttpGet("dependencies/candidates")]
  [Consumes(MediaTypeNames.Application.Json)]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(typeof(DependencyManifest), (int)HttpStatusCode.OK)]
  public Task<DependencyManifest> GetCandidateDependencies(
      [Required, FromBody, MinLength(1)] List<PluginDependency> dependencies) {
    return _pluginService.GetPossibleVersions(dependencies);
  }
  
}