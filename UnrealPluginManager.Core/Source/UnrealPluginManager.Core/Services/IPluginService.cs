using System.IO.Abstractions;
using LanguageExt;
using Retro.SimplePage;
using Semver;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Storage;

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
  /// Adds a new plugin to the system using the provided plugin name and descriptor information.
  /// </summary>
  /// <param name="pluginName">
  ///   The unique name of the plugin to be added.
  /// </param>
  /// <param name="descriptor">
  ///   A <see cref="PluginDescriptor"/> object containing detailed metadata about the plugin, such as version, description, and other attributes.
  /// </param>
  /// <param name="fileData"></param>
  /// <returns>
  /// A <see cref="PluginSummary"/> representing the added plugin, including its name and optional description.
  /// </returns>
  Task<PluginVersionDetails> AddPlugin(string pluginName, PluginDescriptor descriptor,
                                       PartitionedPlugin fileData);

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
  /// Submits a plugin file for processing and storage, associating it with a specific engine version.
  /// </summary>
  /// <param name="fileData">
  ///     The stream containing the plugin file data to be submitted.
  /// </param>
  /// <param name="engineVersion">
  ///     The version of the Unreal Engine that the plugin is associated with.
  /// </param>
  /// <returns>
  /// A <see cref="PluginSummary"/> representing the processed and stored plugin, including its metadata.
  /// </returns>
  Task<PluginVersionDetails> SubmitPlugin(Stream fileData, string engineVersion);

  /// <summary>
  /// Submits a plugin along with its platform-specific binaries.
  /// </summary>
  /// <param name="submission"></param>
  /// <returns>
  /// A <see cref="PluginDetails"/> object containing detailed information about the submitted plugin.
  /// </returns>
  Task<PluginVersionDetails> SubmitPlugin(Stream submission);

  /// <summary>
  /// Submits a plugin from the specified directory for inclusion in the system, including processing metadata and versioning information.
  /// </summary>
  /// <param name="pluginDirectory">The directory containing the plugin's files, including its descriptor.</param>
  /// <param name="engineVersion">The version of the engine that the plugin supports.</param>
  /// <returns>
  /// A <see cref="PluginDetails"/> object containing detailed information about the submitted plugin.
  /// </returns>
  Task<PluginVersionDetails> SubmitPlugin(IDirectoryInfo pluginDirectory, string engineVersion);

  /// <summary>
  /// Retrieves the source code file for a specific plugin version.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin.</param>
  /// <param name="versionId">The unique identifier of the plugin version.</param>
  /// <returns>
  /// An <see cref="IFileInfo"/> representing the source code file of the specified plugin version.
  /// </returns>
  Task<IFileInfo> GetPluginSource(Guid pluginId, Guid versionId);

  /// <summary>
  /// Retrieves the binary data of a specified plugin for a specific version, engine version, and platform.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin.</param>
  /// <param name="versionId">The unique identifier of the specific version of the plugin.</param>
  /// <param name="engineVersion">The engine version the plugin is associated with.</param>
  /// <param name="platform">The target platform for which the plugin binary is requested.</param>
  /// <returns>
  /// A <see cref="IFileInfo"/> representing the binary data file of the requested plugin.
  /// </returns>
  Task<IFileInfo> GetPluginBinaries(Guid pluginId, Guid versionId, string engineVersion,
                                    string platform);


  /// <summary>
  /// Retrieves the stored plugin version data, including the source file, optional icon, and list of binaries for the specified plugin and version.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin.</param>
  /// <param name="versionId">The unique identifier of the plugin version.</param>
  /// <returns>
  /// A <see cref="StoredPluginVersion"/> containing the source file, optional icon, and binaries for the specified plugin version.
  /// </returns>
  Task<PluginDownload> GetPluginFileData(Guid pluginId, Guid versionId);


  /// <summary>
  /// Retrieves the raw file data for a specified plugin, based on the provided plugin identifier, version range, engine version, and target platforms.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin to retrieve.</param>
  /// <param name="targetVersion">The range of plugin versions to consider when fetching the file data.</param>
  /// <param name="engineVersion">The specific engine version that the plugin file should be compatible with.</param>
  /// <param name="targetPlatforms">The collection of target platform identifiers for which the plugin file is relevant.</param>
  /// <param name="separated"></param>
  /// <returns>
  /// A <see cref="Stream"/> containing the requested plugin file data.
  /// </returns>
  Task<PluginDownload> GetPluginFileData(Guid pluginId, SemVersionRange targetVersion, string engineVersion,
                                         IReadOnlyCollection<string> targetPlatforms, bool separated = false);

  /// <summary>
  /// Retrieves the file data of a plugin version for a specified engine version and target platforms.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin.</param>
  /// <param name="versionId">The unique identifier of the specific version of the plugin.</param>
  /// <param name="engineVersion">The engine version for which the plugin file data is requested.</param>
  /// <param name="targetPlatforms">The collection of target platforms for which the plugin file is intended.</param>
  /// <param name="separated"></param>
  /// <returns>
  /// A stream containing the binary content of the plugin file.
  /// </returns>
  Task<PluginDownload> GetPluginFileData(Guid pluginId, Guid versionId, string engineVersion,
                                         IReadOnlyCollection<string> targetPlatforms, bool separated = false);

  /// <summary>
  /// Retrieves a collection of plugin data files that match the specified criteria including plugin name, version, engine version, and target platforms.
  /// </summary>
  /// <param name="pluginName">The name of the plugin to retrieve data for.</param>
  /// <param name="pluginVersion">The version of the plugin to retrieve data for.</param>
  /// <param name="engineVersion">The version of the engine for which the plugin is targeted.</param>
  /// <param name="targetPlatforms">A collection of target platforms for the plugin.</param>
  /// <returns>
  /// An asynchronous enumerable collection of <see cref="IFileInfo"/> representing plugin data files.
  /// </returns>
  IAsyncEnumerable<IFileInfo> GetAllPluginData(string pluginName, SemVersion pluginVersion, string engineVersion,
                                               IReadOnlyCollection<string> targetPlatforms);


}