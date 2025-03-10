using System.Collections;
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
using UnrealPluginManager.Server.Model.Files;

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
  /// Submits a plugin for processing by uploading source code and a collection of binaries.
  /// </summary>
  /// <param name="submission">An object containing the plugin's source code file and associated binaries for submission.</param>
  /// <return>Returns the details of the submitted plugin upon successful processing.</return>
  [HttpPost]
  [Consumes(MediaTypeNames.Multipart.FormData)]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(typeof(PluginDetails), (int)HttpStatusCode.OK)]
  public async Task<PluginDetails> SubmitPlugin([FromForm] PluginSubmission submission) {
    await using var sourceStream = submission.SourceCode.OpenReadStream();
    await using var binariesStreams = submission.Binaries
        .ToAsyncDisposableDictionary(x => x.Key.ToString(), 
                                     x => x.Value
                                         .ToAsyncDisposableDictionary(y => y.Key, 
                                                                      y => y.Value.OpenReadStream()));
    return await _pluginService.SubmitPlugin(sourceStream, binariesStreams);
  }

  /// <summary>
  /// Retrieves a paginated list of the latest plugin versions filtered by the specified criteria.
  /// </summary>
  /// <param name="match">A wildcard string used to filter plugins by name. Defaults to "*".</param>
  /// <param name="versionRange">The semantic version range to filter the plugin versions. Defaults to all release versions.</param>
  /// <param name="pageable">Pagination settings including page size, index, and sort order.</param>
  /// <return>Returns a paginated collection of the latest plugin version information.</return>
  [HttpGet("latest")]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(typeof(Page<PluginVersionInfo>), (int)HttpStatusCode.OK)]
  public Task<Page<PluginVersionInfo>> GetLatestVersions([FromQuery] string match = "*",
                                                         [FromQuery] SemVersionRange? versionRange = null,
                                                         [FromQuery] Pageable pageable = default) {
    return _pluginService.ListLatestedVersions(match, versionRange ?? SemVersionRange.AllRelease, pageable);
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
  /// <param name="pluginId">The unique identifier of the plugin to retrieve the latest version for.</param>
  /// <param name="version">An optional version range to filter the plugin's versions. Defaults to all released versions.</param>
  /// <return>Returns details of the latest version of the specified plugin that satisfies the given version range.</return>
  [HttpGet("{pluginId:guid}/latest")]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(typeof(PluginVersionInfo), (int)HttpStatusCode.OK)]
  public async Task<PluginVersionInfo> GetLatestVersion([FromRoute] Guid pluginId,
                                                        [FromQuery] SemVersionRange? version = null) {
    var latest = await _pluginService.GetPluginVersionInfo(pluginId, version ?? SemVersionRange.AllRelease);
    return latest.OrElseThrow(
        () => new PluginNotFoundException(
            $"Could not find plugin {pluginId} that satisfies the specified version range."));
  }

  /// <summary>
  /// Retrieves the dependency tree for a specified plugin.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin whose dependency tree is to be retrieved.</param>
  /// <param name="targetVersion">An optional version range used to filter dependencies for the plugin.</param>
  /// <return>Returns a list of plugin summaries representing the dependency tree.</return>
  [HttpGet("{pluginId:guid}/latest/dependencies")]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(typeof(List<PluginSummary>), (int)HttpStatusCode.OK)]
  public Task<List<PluginSummary>> GetDependencyTree([FromRoute] Guid pluginId, SemVersionRange? targetVersion = null) {
    return _pluginService.GetDependencyList(pluginId, targetVersion);
  }

  /// <summary>
  /// Downloads a plugin file as a ZIP archive for the specified plugin, engine version, and target platforms.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin to be downloaded.</param>
  /// <param name="engineVersion">The Unreal Engine version for which the plugin file is requested.</param>
  /// <param name="targetVersion">The semantic version range that specifies the version of the plugin to target. Defaults to all release versions if not specified.</param>
  /// <param name="platforms">The collection of target platforms for which the plugin file is compatible.</param>
  /// <return>Returns a FileStreamResult containing the plugin file as a ZIP archive.</return>
  [HttpGet("{pluginId:guid}/latest/{engineVersion}/download")]
  [Produces(MediaTypeNames.Application.Zip)]
  [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
  public async Task<FileStreamResult> DownloadLatestPlugin([FromRoute] Guid pluginId,
                                                           [FromRoute] Version engineVersion,
                                                           [FromQuery] SemVersionRange? targetVersion,
                                                           [FromQuery] IReadOnlyCollection<string> platforms) {
    return File(
        await _pluginService.GetPluginFileData(pluginId, targetVersion ?? SemVersionRange.AllRelease,
                                               engineVersion.ToString(), platforms),
        MediaTypeNames.Application.Zip,
        $"{pluginId}.zip");
  }


  /// <summary>
  /// Downloads the specified version of a plugin as a ZIP file for the specified Unreal Engine version and target platforms.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin to download.</param>
  /// <param name="versionId">The unique identifier of the plugin version to download.</param>
  /// <param name="engineVersion">The version of Unreal Engine compatible with the plugin.</param>
  /// <param name="platforms">The collection of target platforms for the plugin.</param>
  /// <return>Returns a ZIP file containing the requested plugin version for the specified engine version and platforms.</return>
  [HttpGet("{pluginId:guid}/{versionId:guid}/download/{engineVersion}")]
  [Produces(MediaTypeNames.Application.Zip)]
  [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
  public async Task<FileStreamResult> DownloadPluginVersion([FromRoute] Guid pluginId, [FromRoute] Guid versionId,
                                                            [FromRoute] Version engineVersion,
                                                            [FromQuery] IReadOnlyCollection<string> platforms) {
    return File(
        await _pluginService.GetPluginFileData(pluginId, versionId, engineVersion.ToString(), platforms),
        MediaTypeNames.Application.Zip,
        $"{pluginId}.zip");
  }

  /// <summary>
  /// Downloads the source code of a specific plugin version as a zip file.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin to download.</param>
  /// <param name="versionId">The unique identifier of the specific version of the plugin to download.</param>
  /// <return>Returns a file stream result containing the plugin source code in a zip archive.</return>
  [HttpGet("{pluginId:guid}/{versionId:guid}/download/source")]
  [Produces(MediaTypeNames.Application.Zip)]
  [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
  public async Task<FileStreamResult> DownloadPluginSource([FromRoute] Guid pluginId, [FromRoute] Guid versionId) {
    return File(
        await _pluginService.GetPluginSource(pluginId, versionId).Map(x => x.OpenRead()),
        MediaTypeNames.Application.Zip,
        $"{pluginId}.zip");
  }

  /// <summary>
  /// Downloads the binary files of a specified plugin for a given version, engine version, and platform.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin whose binaries are being downloaded.</param>
  /// <param name="versionId">The unique identifier of the plugin version to download binaries for.</param>
  /// <param name="engineVersion">The engine version for which the plugin binaries are compatible.</param>
  /// <param name="platform">The platform for which the plugin binaries are compiled.</param>
  /// <return>Returns the binary files of the specified plugin as a file stream in ZIP format.</return>
  [HttpGet("{pluginId:guid}/{versionId:guid}/download/{engineVersion}/{platform}/binaries")]
  [Produces(MediaTypeNames.Application.Zip)]
  [ProducesResponseType(typeof(FileStreamResult), (int)HttpStatusCode.OK)]
  public async Task<FileStreamResult> DownloadPluginBinaries([FromRoute] Guid pluginId, [FromRoute] Guid versionId,
                                                             [FromRoute] Version engineVersion,
                                                             [FromRoute] string platform) {
    return File(
        await _pluginService.GetPluginBinaries(pluginId, versionId, engineVersion.ToString(), platform).Map(x => x.OpenRead()),
        MediaTypeNames.Application.Zip,
        $"{pluginId}.zip");
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