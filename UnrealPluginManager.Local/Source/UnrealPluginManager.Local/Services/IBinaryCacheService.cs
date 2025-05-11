using System.IO.Abstractions;
using LanguageExt;
using Semver;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Local.Model.Cache;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// Represents a service for managing cached binary builds of Unreal Engine plugins.
/// </summary>
public interface IBinaryCacheService {

  /// Caches a built plugin into a binary cache, ensuring that the plugin's build information
  /// and associated files are stored for future retrieval.
  /// <param name="manifest">The manifest containing metadata and configuration of the plugin to be cached.</param>
  /// <param name="directory">The directory containing the built plugin files.</param>
  /// <param name="patches">A list of patch identifiers applied to the plugin during building.</param>
  /// <param name="engineVersion">The version of the engine used to build the plugin.</param>
  /// <param name="platforms">The collection of platforms for which the plugin was built.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains
  /// a <see cref="PluginBuildInfo"/> object with details about the cached build.</returns>
  Task<PluginBuildInfo> CacheBuiltPlugin(PluginManifest manifest, IDirectoryInfo directory,
                                         IReadOnlyList<string> patches,
                                         string engineVersion, IReadOnlyCollection<string> platforms);

  /// Retrieves a cached build of a plugin based on name, version, engine version, and target platforms.
  /// <param name="pluginName">The name of the plugin to retrieve from the cache.</param>
  /// <param name="pluginVersion">The version of the plugin to retrieve from the cache.</param>
  /// <param name="engineVersion">The version of the engine for which the plugin was built.</param>
  /// <param name="targetPlatforms">The collection of platforms for which the plugin was built.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains an optional <see cref="PluginBuildInfo"/> object with details about the cached plugin build, or none if no match is found in the cache.</returns>
  Task<Option<PluginBuildInfo>> GetCachedPluginBuild(string pluginName, SemVersion pluginVersion, string engineVersion,
                                                     IReadOnlyCollection<string> targetPlatforms);

  /// Retrieves the directory of a cached plugin build, if it exists, based on the provided plugin name,
  /// version, engine version, and target platforms.
  /// <param name="pluginName">The name of the plugin whose cached build is being queried.</param>
  /// <param name="pluginVersion">The semantic version of the plugin.</param>
  /// <param name="engineVersion">The version of the engine associated with the cached build.</param>
  /// <param name="targetPlatforms">The collection of target platforms for which the plugin was built.</param>
  /// <returns>An option containing the directory of the cached plugin build if it exists, otherwise none.</returns>
  Task<Option<IDirectoryInfo>> GetCachedBuildDirectory(string pluginName, SemVersion pluginVersion,
                                                       string engineVersion,
                                                       IReadOnlyCollection<string> targetPlatforms);

}