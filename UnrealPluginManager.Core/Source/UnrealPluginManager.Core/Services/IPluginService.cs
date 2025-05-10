using LanguageExt;
using Retro.SimplePage;
using Semver;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Plugins.Recipes;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Service declaration for operations involving plugins.
/// </summary>
public interface IPluginService {
  /// <summary>
  /// Retrieves a collection of plugin summaries including essential information like name and optional description.
  /// </summary>
  /// <param name="matcher"></param>
  /// <param name="pageable">The pagination settings</param>
  /// <returns>
  /// An enumerable collection of <see cref="PluginSummary"/> representing the summaries of all plugins.
  /// </returns>
  Task<Page<PluginOverview>> ListPlugins(string matcher = "", Pageable pageable = default);

  /// <summary>
  /// Retrieves the latest versions of a specific plugin filtered by a version range and paginated settings.
  /// </summary>
  /// <param name="pluginName">The name of the plugin for which the latest versions are requested.</param>
  /// <param name="versionRange">The semantic version range used to filter the plugin versions.</param>
  /// <param name="pageable">The pagination settings to control the size and offset of the result set.</param>
  /// <returns>
  /// A <see cref="Page{PluginVersionInfo}"/> containing the filtered and paginated plugin version information.
  /// </returns>
  Task<Page<PluginVersionInfo>> ListLatestVersions(string pluginName, SemVersionRange versionRange,
                                                   Pageable pageable = default);

  /// <summary>
  /// Retrieves detailed information about a specific plugin, including its versions and metadata.
  /// </summary>
  /// <param name="pluginName">The name of the plugin for which details are to be retrieved.</param>
  /// <param name="version">The specific version of the plugin to retrieve details for.</param>
  /// <returns>
  /// An <see cref="Option{PluginDetails}"/> containing the plugin details if found; otherwise, an empty option.
  /// </returns>
  Task<Option<PluginVersionDetails>> GetPluginVersionDetails(string pluginName, SemVersion version);

  /// <summary>
  /// Retrieves detailed version information associated with a specific plugin name and version range.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin.</param>
  /// <param name="versionRange">The range of versions to filter applicable plugin versions.</param>
  /// <returns>
  /// An optional <see cref="PluginVersionInfo"/> containing detailed version information
  /// if a matching plugin version is found; otherwise, an empty option.
  /// </returns>
  Task<Option<PluginVersionInfo>> GetPluginVersionInfo(Guid pluginId, SemVersionRange versionRange);

  /// <summary>
  /// Retrieves detailed information about a specific plugin version based on its name and version range.
  /// </summary>
  /// <param name="pluginName">The name of the plugin to retrieve version information for.</param>
  /// <param name="versionRange">The version range to search within for the plugin.</param>
  /// <returns>
  /// An <see cref="Option{PluginVersionInfo}"/> containing the version information for the plugin,
  /// if a match is found within the specified version range.
  /// </returns>
  Task<Option<PluginVersionInfo>> GetPluginVersionInfo(string pluginName, SemVersionRange versionRange);

  /// <summary>
  /// Retrieves version information for a specific plugin based on its name and version.
  /// </summary>
  /// <param name="pluginName">The name of the plugin to retrieve version information for.</param>
  /// <param name="version">The specific semantic version of the plugin.</param>
  /// <returns>
  /// An <see cref="Option{PluginVersionInfo}"/> containing the version details of the plugin if found, otherwise an empty option.
  /// </returns>
  Task<Option<PluginVersionInfo>> GetPluginVersionInfo(string pluginName, SemVersion version);

  /// <summary>
  /// Determines and retrieves possible versions of plugins that satisfy the given list of dependencies.
  /// </summary>
  /// <param name="dependencies">A list of plugin dependencies for which to find compatible versions.</param>
  /// <returns>
  /// A task that represents the asynchronous operation. The task result contains a <see cref="DependencyManifest"/>
  /// outlining the resolved possible versions for the specified dependencies.
  /// </returns>
  Task<DependencyManifest> GetPossibleVersions(List<PluginDependency> dependencies);

  /// <summary>
  /// Retrieves the list of dependencies for a specified plugin, including both direct and transitive dependencies.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin for which dependencies are to be retrieved.</param>
  /// <param name="targetVersion">An optional version range filter to limit the retrieved dependencies based on specific version constraints.</param>
  /// <returns>
  /// A task that represents the asynchronous operation, containing a list of <see cref="PluginSummary"/> objects representing the dependencies of the specified plugin.
  /// </returns>
  Task<List<PluginSummary>> GetDependencyList(Guid pluginId, SemVersionRange? targetVersion = null);

  /// <summary>
  /// Retrieves a list of plugin summaries based on the specified dependency chain root and dependency manifest.
  /// </summary>
  /// <param name="root">The root of the dependency chain for which the plugin summaries should be retrieved.</param>
  /// <param name="manifest">The manifest containing possible versions for the dependencies.</param>
  /// <returns>
  /// A list of <see cref="PluginSummary"/> representing the plugins identified in the dependency chain.
  /// </returns>
  List<PluginSummary> GetDependencyList(IDependencyChainNode root, DependencyManifest manifest);

  /// <summary>
  /// Retrieves the README file content for a specific plugin version.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin.</param>
  /// <param name="versionId">The unique identifier of the specific plugin version.</param>
  /// <returns>
  /// A string containing the content of the README file associated with the specified plugin version.
  /// </returns>
  Task<string> GetPluginReadme(Guid pluginId, Guid versionId);

  /// <summary>
  /// Adds a README file for a specific plugin version.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin.</param>
  /// <param name="versionId">The unique identifier for the specific version of the plugin.</param>
  /// <param name="readme">The content of the README file to be added.</param>
  /// <returns>
  /// The content of the README file after it has been successfully added.
  /// </returns>
  Task<string> AddPluginReadme(Guid pluginId, Guid versionId, string readme);

  /// <summary>
  /// Updates the readme content for a specific plugin version.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin.</param>
  /// <param name="versionId">The unique identifier of the plugin version.</param>
  /// <param name="readme">The updated readme content to be associated with the plugin version.</param>
  /// <returns>
  /// The updated readme content as a string.
  /// </returns>
  Task<string> UpdatePluginReadme(Guid pluginId, Guid versionId, string readme);

  /// <summary>
  /// Submits a plugin to the system by providing its manifest, optional icon, and optional readme file.
  /// </summary>
  /// <param name="manifest">The manifest containing the core details and metadata of the plugin.</param>
  /// <param name="icon">An optional stream containing the plugin's representation icon.</param>
  /// <param name="readme">An optional readme text containing additional information about the plugin.</param>
  /// <returns>
  /// The details of the submitted plugin version as a <see cref="PluginVersionDetails"/> object.
  /// </returns>
  Task<PluginVersionDetails> SubmitPlugin(PluginManifest manifest, Stream? icon = null, string? readme = null);


}