using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Retro.SimplePage;
using Semver;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Server.Auth;
using UnrealPluginManager.Server.Auth.ApiKey;
using UnrealPluginManager.Server.Model;

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
[Route("plugins")]
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
  public Task<Page<PluginOverview>> GetPlugins([FromQuery] string match = "",
                                               [FromQuery] Pageable pageable = default) {
    return _pluginService.ListPlugins(match, pageable);
  }

  /// <summary>
  /// Submits a new plugin version along with optional icon and README information.
  /// </summary>
  /// <param name="manifest">The manifest containing metadata and configuration details for the plugin.</param>
  /// <param name="icon">An optional image file to represent the plugin's icon.</param>
  /// <param name="readme">Optional README content providing additional information about the plugin.</param>
  /// <return>Returns the details of the submitted plugin version.</return>
  [HttpPost]
  [Consumes(MediaTypeNames.Multipart.FormData)]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(typeof(PluginVersionInfo), (int) HttpStatusCode.OK)]
  [ApiKey]
  [Authorize(AuthorizationPolicies.CanSubmitPlugin)]
  public async Task<PluginVersionInfo> SubmitPlugin(PluginSubmissionMultipartRequest requestBody) {
    await using var iconStream = requestBody.Icon?.OpenReadStream();
    return await _pluginService.SubmitPlugin(requestBody.Manifest, iconStream, requestBody.Readme);
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
  [ProducesResponseType(typeof(Page<PluginVersionInfo>), (int) HttpStatusCode.OK)]
  public Task<Page<PluginVersionInfo>> GetLatestVersions([FromQuery] string match = "",
                                                         [FromQuery] SemVersionRange? versionRange = null,
                                                         [FromQuery] Pageable pageable = default) {
    return _pluginService.ListLatestVersions(match, versionRange ?? SemVersionRange.AllRelease, pageable);
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
  [ProducesResponseType(typeof(PluginVersionInfo), (int) HttpStatusCode.OK)]
  public async Task<PluginVersionInfo> GetLatestVersion([FromRoute] Guid pluginId,
                                                        [FromQuery] SemVersionRange? version = null) {
    var latest = await _pluginService.GetPluginVersionInfo(pluginId, version ?? SemVersionRange.AllRelease);
    return latest.OrElseThrow(() => new PluginNotFoundException(
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
  [ProducesResponseType(typeof(List<PluginSummary>), (int) HttpStatusCode.OK)]
  public Task<List<PluginSummary>> GetDependencyTree([FromRoute] Guid pluginId, SemVersionRange? targetVersion = null) {
    return _pluginService.GetDependencyList(pluginId, targetVersion);
  }

  /// <summary>
  /// Retrieves the readme content for a specific version of a plugin.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin.</param>
  /// <param name="versionId">The unique identifier of the plugin version.</param>
  /// <return>Returns the readme content as a markdown-formatted string.</return>
  [HttpGet("{pluginId:guid}/{versionId:guid}/readme")]
  [Produces(MediaTypeNames.Text.Markdown)]
  [ProducesResponseType(typeof(string), (int) HttpStatusCode.OK)]
  public Task<string> GetPluginReadme([FromRoute] Guid pluginId, [FromRoute] Guid versionId) {
    return _pluginService.GetPluginReadme(pluginId, versionId);
  }

  /// <summary>
  /// Adds or updates the README content for the specified plugin version.
  /// </summary>
  /// <param name="pluginId">The unique identifier for the plugin.</param>
  /// <param name="versionId">The unique identifier for the specific version of the plugin.</param>
  /// <param name="readme">The README content in markdown format to be added or updated.</param>
  /// <return>Returns the updated README content as a string.</return>
  [HttpPost("{pluginId:guid}/{versionId:guid}/readme")]
  [Consumes(MediaTypeNames.Text.Markdown, MediaTypeNames.Application.Json)]
  [Produces(MediaTypeNames.Text.Markdown)]
  [ProducesResponseType(typeof(string), (int) HttpStatusCode.OK)]
  [Authorize(AuthorizationPolicies.CanEditPlugin)]
  [ApiKey]
  public Task<string> AddPluginReadme([FromRoute] Guid pluginId, [FromRoute] Guid versionId,
                                      [FromBody] string readme) {
    return _pluginService.AddPluginReadme(pluginId, versionId, readme);
  }

  /// <summary>
  /// Updates the README content for a specific plugin version.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin whose README is being updated.</param>
  /// <param name="versionId">The unique identifier for the specific version of the plugin.</param>
  /// <param name="readme">The new README content to replace the existing one.</param>
  /// <return>Returns the updated README content as a string.</return>
  [HttpPut("{pluginId:guid}/{versionId:guid}/readme")]
  [Consumes(MediaTypeNames.Text.Markdown, MediaTypeNames.Application.Json)]
  [Produces(MediaTypeNames.Text.Markdown)]
  [ProducesResponseType(typeof(string), (int) HttpStatusCode.OK)]
  [Authorize(AuthorizationPolicies.CanEditPlugin)]
  [ApiKey]
  public Task<string> UpdatePluginReadme([FromRoute] Guid pluginId, [FromRoute] Guid versionId,
                                         [FromBody] string readme) {
    return _pluginService.UpdatePluginReadme(pluginId, versionId, readme);
  }

  /// <summary>
  /// Retrieves a dependency manifest containing potential versions for the given list of plugin dependencies.
  /// </summary>
  /// <param name="dependencies">A list of plugin dependencies for which potential versions are to be determined.</param>
  /// <return>Returns a dependency manifest with possible versions for the specified dependencies.</return>
  [HttpGet("dependencies/candidates")]
  [Consumes(MediaTypeNames.Application.Json)]
  [Produces(MediaTypeNames.Application.Json)]
  [ProducesResponseType(typeof(DependencyManifest), (int) HttpStatusCode.OK)]
  public Task<DependencyManifest> GetCandidateDependencies(
      [Required, FromBody, MinLength(1)] List<PluginDependency> dependencies) {
    return _pluginService.GetPossibleVersions(dependencies);
  }

}