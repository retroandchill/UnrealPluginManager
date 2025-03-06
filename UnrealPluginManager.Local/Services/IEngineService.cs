using System.IO.Abstractions;
using LanguageExt;
using Semver;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Local.Model.Engine;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// Provides functionalities related to engine management within the Unreal Plugin Manager CLI.
/// </summary>
public interface IEngineService {
  /// <summary>
  /// Retrieves a list of installed Unreal Engine versions available on the system.
  /// </summary>
  /// <returns>
  /// A list of <see cref="InstalledEngine"/> objects representing the installed Unreal Engine versions.
  /// </returns>
  List<InstalledEngine> GetInstalledEngines();

  /// <summary>
  /// Builds a specified Unreal Engine plugin using the provided engine version.
  /// </summary>
  /// <param name="pluginFile">
  ///     The <see cref="IFileInfo"/> representing the path of the plugin file to be built.
  /// </param>
  /// <param name="engineVersion">
  ///     The version of the Unreal Engine to use for building the plugin. If null, the default engine version is used.
  /// </param>
  /// <returns>
  /// A task representing the asynchronous operation with an integer exit code indicating the result of the build process.
  /// </returns>
  public Task<int> BuildPlugin(IFileInfo pluginFile, string? engineVersion);

  /// <summary>
  /// Retrieves the installed version of a specified plugin for a given Unreal Engine version, if available.
  /// </summary>
  /// <param name="pluginName">
  /// The name of the plugin for which the version is being retrieved.
  /// </param>
  /// <param name="engineVersion">
  /// The version of the Unreal Engine to check for the installed plugin. This parameter can be null.
  /// </param>
  /// <returns>
  /// An <see cref="Option{T}"/> containing the installed <see cref="SemVersion"/> of the plugin if found; otherwise, an empty option.
  /// </returns>
  public Task<Option<SemVersion>> GetInstalledPluginVersion(string pluginName, string? engineVersion);

  /// <summary>
  /// Retrieves a list of installed plugins for a specified Unreal Engine version.
  /// </summary>
  /// <param name="engineVersion">
  ///   The version of the Unreal Engine for which to retrieve the installed plugins.
  ///   If null, the default engine version will be used.
  /// </param>
  /// <returns>
  /// An asynchronous enumerable of <see cref="PluginIdentifier"/> objects representing the plugins installed for the specified engine version.
  /// </returns>
  public IAsyncEnumerable<PluginIdentifier> GetInstalledPlugins(string? engineVersion);

  /// <summary>
  /// Installs the specified plugin for a given Unreal Engine version.
  /// </summary>
  /// <param name="pluginName">The name of the plugin to install.</param>
  /// <param name="pluginVersion">The version of the plugin to install, specified as a semantic version range.</param>
  /// <param name="engineVersion">The target Unreal Engine version for the installation, or null to use the default.</param>
  /// <param name="targetPlatforms"></param>
  /// <returns>
  /// A task representing the asynchronous operation. The task result contains an integer indicating the status of the installation process.
  /// </returns>
  public Task<int> InstallPlugin(string pluginName, SemVersion pluginVersion, string? engineVersion,
                                 IReadOnlyCollection<string> targetPlatforms);
}