﻿using LanguageExt;
using Semver;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Local.Model.Cache;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// Represents a service used for remote calls to fetch plugin data.
/// </summary>
public interface IPluginManagementService {
  /// <summary>
  /// Searches for a locally stored plugin matching the specified name, version range, engine version, and platforms.
  /// </summary>
  /// <param name="pluginName">
  /// The name of the plugin to search for. This is a required string identifying the plugin.
  /// </param>
  /// <param name="versionRange">
  /// The semantic version range that the plugin must match.
  /// </param>
  /// <param name="engineVersion">
  /// The specific version of the engine for which the plugin is sought.
  /// </param>
  /// <param name="platforms">
  /// A collection of platform identifiers that the plugin must support.
  /// </param>
  /// <returns>
  /// A task representing the asynchronous operation. Upon completion, returns an <see cref="Option{T}"/>
  /// containing a <see cref="PluginBuildInfo"/> if a matching plugin is found, or <see cref="Option{T}.None"/> if no match exists.
  /// </returns>
  Task<Option<PluginBuildInfo>> FindLocalPlugin(string pluginName, SemVersion versionRange, string engineVersion,
                                                IReadOnlyCollection<string> platforms);

  /// <summary>
  /// Retrieves a list of plugin overviews that optionally match a given search term.
  /// The method aggregates and groups plugins by name, merging relevant details
  /// from different sources if applicable.
  /// </summary>
  /// <param name="searchTerm">
  /// An optional string used to filter plugins by matching their names or other attributes.
  /// If null or empty, all available plugins are returned.
  /// </param>
  /// <returns>
  /// A task representing the asynchronous operation. Upon completion, returns an
  /// <see cref="OrderedDictionary{TKey, TValue}"/> where each key is a plugin identifier and each value
  /// is a <see cref="Fin{T}"/> containing a list of <see cref="PluginOverview"/> objects.
  /// </returns>
  Task<OrderedDictionary<string, Fin<List<PluginOverview>>>> GetPlugins(string searchTerm);

  /// <summary>
  /// Retrieves a list of plugin overviews from the specified remote source that optionally match a given search term.
  /// The method queries the remote API and fetches relevant plugin details filtered by the search term if provided.
  /// </summary>
  /// <param name="remote">
  /// A string representing the identifier of the remote source where the plugins are fetched from.
  /// This value determines the specific remote API to query for plugins.
  /// </param>
  /// <param name="searchTerm">
  /// An optional string used to filter plugins by matching their names or other attributes.
  /// If null or empty, all available plugins from the specified remote source are returned.
  /// </param>
  /// <returns>
  /// A task representing the asynchronous operation. Upon completion, returns a
  /// list of <see cref="PluginOverview"/> objects containing detailed information about the plugins retrieved.
  /// </returns>
  Task<List<PluginOverview>> GetPlugins(string remote, string searchTerm);

  /// <summary>
  /// Finds and retrieves the target version information for a specified plugin
  /// that matches the given version range and engine version, either by checking
  /// installed plugins or looking up available versions.
  /// </summary>
  /// <param name="pluginName">
  /// The name of the plugin to locate. This must match the plugin's identifier.
  /// </param>
  /// <param name="versionRange">
  /// The range of acceptable versions for the plugin. If multiple versions are found to satisfy the range,
  /// prioritization will occur based on specific criteria.
  /// </param>
  /// <param name="engineVersion">
  /// An optional engine version used to filter plugins that are compatible with the specified engine version.
  /// If null, compatibility with any engine version is assumed.
  /// </param>
  /// <returns>
  /// A task representing the asynchronous operation. Upon completion, returns a
  /// <see cref="PluginVersionInfo"/> object containing details about the resolved plugin version.
  /// Throws a <see cref="PluginNotFoundException"/> if no matching plugin is found.
  /// </returns>
  Task<PluginVersionInfo> FindTargetPlugin(string pluginName, SemVersionRange versionRange, string? engineVersion);

  /// <summary>
  /// Retrieves a list of plugins that need to be installed based on the provided root dependency chain node
  /// and the specified engine version.
  /// </summary>
  /// <param name="root">
  ///   The root node of the dependency chain that defines the plugin and its dependencies.
  /// </param>
  /// <param name="engineVersion">
  ///   The version of the game engine, which is used to determine compatibility with plugins.
  ///   This can be null if engine version filtering is not required.
  /// </param>
  /// <returns>
  /// A task representing the asynchronous operation. Upon completion, returns a <see cref="List{PluginSummary}"/>
  /// containing the plugins that need to be installed, including their details.
  /// </returns>
  Task<List<PluginSummary>> GetPluginsToInstall(IDependencyChainNode root, string? engineVersion);

  /// <summary>
  /// Uploads a plugin with the specified name and version to a given remote.
  /// The method retrieves the plugin's data, including its binaries and icon (if available),
  /// and submits it to the designated or default remote location.
  /// </summary>
  /// <param name="pluginName">
  /// The name of the plugin to upload.
  /// </param>
  /// <param name="version">
  /// The version of the plugin to upload, represented as a <see cref="SemVersion"/> object.
  /// </param>
  /// <param name="remote">
  /// An optional string specifying the name of the remote destination for the plugin upload.
  /// If null, the default remote is used.
  /// </param>
  /// <returns>
  /// A task representing the asynchronous operation. Upon completion, returns a <see cref="PluginVersionInfo"/>
  /// object containing detailed information about the uploaded plugin.
  /// </returns>
  Task<PluginVersionInfo> UploadPlugin(string pluginName, SemVersion version, string? remote);

  /// <summary>
  /// Downloads the specified version of a plugin from a remote source
  /// and retrieves its build information, such as compatibility and platform details.
  /// </summary>
  /// <param name="pluginName">
  /// The name of the plugin to be downloaded. This must not be null or empty.
  /// </param>
  /// <param name="version">
  /// The semantic version of the plugin to be downloaded.
  /// </param>
  /// <param name="remote">
  /// An optional identifier for the remote source from which to download the plugin.
  /// If null, a default source may be utilized.
  /// </param>
  /// <param name="engineVersion">
  /// The specific version of the engine for which the plugin is being retrieved.
  /// This value ensures compatibility with the desired engine version.
  /// </param>
  /// <param name="platforms">
  /// A list of target platforms for which the plugin should be downloaded.
  /// This allows specifying platform-specific plugin requirements.
  /// </param>
  /// <returns>
  /// A task representing the asynchronous operation. Upon completion,
  /// provides a <see cref="PluginBuildInfo"/> object containing the downloaded plugin's
  /// build details, including relevant metadata such as supported platforms and version information.
  /// </returns>
  Task<PluginBuildInfo> DownloadPlugin(string pluginName, SemVersion version, int? remote,
                                       string engineVersion,
                                       List<string> platforms);

  /// <summary>
  /// Builds plugin binaries and metadata using the specified manifest, engine version, and supported platforms.
  /// </summary>
  /// <param name="manifest">
  ///   The manifest containing metadata and configuration for the plugin to be built.
  /// </param>
  /// <param name="patches"></param>
  /// <param name="engineVersion">
  ///   The version of the Unreal Engine to target for the plugin build.
  /// </param>
  /// <param name="platforms">
  ///   The collection of platforms to include in the build process.
  /// </param>
  /// <returns>
  /// A task representing the asynchronous operation. Upon completion, returns a <see cref="PluginBuildInfo"/>
  /// containing details about the successfully built plugin.
  /// </returns>
  Task<PluginBuildInfo> BuildFromManifest(PluginManifest manifest, IReadOnlyList<string> patches, string? engineVersion,
                                          IReadOnlyCollection<string> platforms);
}