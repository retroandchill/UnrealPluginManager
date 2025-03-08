using System.IO.Abstractions;
using LanguageExt;
using Semver;
using UnrealPluginManager.Core.Model.Engine;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Resolution;
using UnrealPluginManager.Core.Pagination;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Represents a plugin with its name and version.
/// </summary>
public record struct PluginIdentifier(string Name, SemVersion Version);

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
  Task<Page<PluginOverview>> ListPlugins(string matcher = "*", Pageable pageable = default);

  /// <summary>
  /// Retrieves version information for a specific plugin by its name and version.
  /// </summary>
  /// <param name="pluginName">The name of the plugin for which version information is requested.</param>
  /// <param name="version">The version of the plugin to retrieve information for.</param>
  /// <returns>
  /// An <see cref="Option{PluginVersionInfo}"/> containing the version information if available,
  /// or an empty result if the plugin version is not found.
  /// </returns>
  Task<Option<PluginVersionInfo>> GetPluginVersionInfo(string pluginName, SemVersion version);

  /// <summary>
  /// Retrieves detailed version information associated with a specific plugin name and version range.
  /// </summary>
  /// <param name="pluginName">The name of the plugin to fetch version information for.</param>
  /// <param name="versionRange">The range of versions to filter applicable plugin versions.</param>
  /// <returns>
  /// An optional <see cref="PluginVersionInfo"/> containing detailed version information
  /// if a matching plugin version is found; otherwise, an empty option.
  /// </returns>
  Task<Option<PluginVersionInfo>> GetPluginVersionInfo(string pluginName, SemVersionRange versionRange);

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
  /// <param name="pluginName">The name of the plugin for which to retrieve the dependency list.</param>
  /// <param name="targetVersion">An optional version range filter for the plugin dependencies.</param>
  /// <returns>
  /// A collection of <see cref="PluginSummary"/> objects representing the dependencies of the specified plugin.
  /// </returns>
  Task<List<PluginSummary>> GetDependencyList(string pluginName, SemVersionRange? targetVersion = null);

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
  ///     The unique name of the plugin to be added.
  /// </param>
  /// <param name="descriptor">
  ///     A <see cref="PluginDescriptor"/> object containing detailed metadata about the plugin, such as version, description, and other attributes.
  /// </param>
  /// <param name="storedFile">The information about the stored file for the plugin</param>
  /// <returns>
  /// A <see cref="PluginSummary"/> representing the added plugin, including its name and optional description.
  /// </returns>
  Task<PluginDetails> AddPlugin(string pluginName, PluginDescriptor descriptor, EngineFileData? storedFile = null);

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
  Task<PluginDetails> SubmitPlugin(Stream fileData, string engineVersion);

  /// <summary>
  /// Submits a plugin from the specified directory for inclusion in the system, including processing metadata and versioning information.
  /// </summary>
  /// <param name="pluginDirectory">The directory containing the plugin's files, including its descriptor.</param>
  /// <param name="engineVersion">The version of the engine that the plugin supports.</param>
  /// <returns>
  /// A <see cref="PluginDetails"/> object containing detailed information about the submitted plugin.
  /// </returns>
  Task<PluginDetails> SubmitPlugin(IDirectoryInfo pluginDirectory, string engineVersion);

  /// <summary>
  /// Retrieves the source code for a specific plugin version.
  /// </summary>
  /// <param name="pluginName">The name of the plugin for which to retrieve the source code.</param>
  /// <param name="targetVersion">The specific version of the plugin to retrieve.</param>
  /// <returns>
  /// A <see cref="Stream"/> representing the source code of the specified plugin version.
  /// </returns>
  Task<IFileInfo> GetPluginSource(string pluginName, SemVersion targetVersion);

  /// <summary>
  /// Retrieves the binary data of a specified plugin for a specific version, engine version, and platform.
  /// </summary>
  /// <param name="pluginName">The name of the plugin for which the binary data is requested.</param>
  /// <param name="targetVersion">The specific version of the plugin to retrieve.</param>
  /// <param name="engineVersion">The engine version the plugin is associated with.</param>
  /// <param name="platform">The target platform for which the plugin binary is requested.</param>
  /// <returns>
  /// A <see cref="Stream"/> containing the binary data of the requested plugin.
  /// </returns>
  Task<IFileInfo> GetPluginBinaries(string pluginName, SemVersion targetVersion, string engineVersion,
                                    string platform);

  /// <summary>
  /// Retrieves the raw file data for a specified plugin, considering the plugin name, version, engine compatibility, and target platforms.
  /// </summary>
  /// <param name="pluginName">The name of the plugin for which the file data is to be retrieved.</param>
  /// <param name="targetVersion">The target plugin version range to fetch the appropriate file data.</param>
  /// <param name="engineVersion">The version of the engine that the plugin file is intended for.</param>
  /// <param name="targetPlatforms">A collection of target platform identifiers for which the plugin is requested.</param>
  /// <returns>
  /// A <see cref="Stream"/> containing the plugin file data.
  /// </returns>
  Task<Stream> GetPluginFileData(string pluginName, SemVersionRange targetVersion, string engineVersion,
                                 IReadOnlyCollection<string> targetPlatforms);

  /// <summary>
  /// Retrieves the file data of a specific plugin for a given engine version.
  /// </summary>
  /// <param name="pluginName">
  ///     The name of the plugin whose file data is to be retrieved.
  /// </param>
  /// <param name="targetVersion"></param>
  /// <param name="engineVersion">
  ///     The version of the engine for which the plugin file data is needed.
  /// </param>
  /// <param name="targetPlatforms"></param>
  /// <returns>
  /// A stream representing the binary content of the plugin file.
  /// </returns>
  Task<Stream> GetPluginFileData(string pluginName, SemVersion targetVersion, string engineVersion,
                                 IReadOnlyCollection<string> targetPlatforms);
  
  IAsyncEnumerable<IFileInfo> GetAllPluginData(string pluginName, SemVersion targetVersion, string engineVersion,
                                               IReadOnlyCollection<string> targetPlatforms);

  /// <summary>
  /// Retrieves all plugin data files matching the specified parameters.
  /// </summary>
  /// <param name="pluginName">The name of the plugin to retrieve data for.</param>
  /// <param name="targetVersion">The version range of the plugin to match.</param>
  /// <param name="engineVersion">The engine version to use when targeting the plugin.</param>
  /// <param name="targetPlatforms">The collection of platforms for which the plugin is targeted.</param>
  /// <returns>
  /// An asynchronous enumerable of <see cref="IFileInfo"/> representing the plugin data files.
  /// </returns>
  IAsyncEnumerable<IFileInfo> GetAllPluginData(string pluginName, SemVersionRange targetVersion, string engineVersion,
                                               IReadOnlyCollection<string> targetPlatforms);
}