using Semver;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Local.Model.Installation;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// Defines methods for installing Unreal Engine plugins and their dependencies.
/// </summary>
public interface IInstallService {
  /// <summary>
  /// Installs an Unreal Engine plugin with the specified version, targeting a specific engine version and platforms.
  /// </summary>
  /// <param name="pluginName">The name of the plugin to install.</param>
  /// <param name="pluginVersion">The semantic version range of the plugin to be installed.</param>
  /// <param name="engineVersion">The Unreal Engine version to target for the plugin installation, or null to target the default engine version.</param>
  /// <param name="platforms">The collection of platforms for which the plugin should be installed.</param>
  /// <returns>A task that represents the asynchronous plugin installation operation. The task result contains the installation result.</returns>
  public Task<List<VersionChange>> InstallPlugin(string pluginName, SemVersionRange pluginVersion,
                                                 string? engineVersion, IReadOnlyCollection<string> platforms);

  /// <summary>
  /// Installs the requirements defined in a descriptor file for Unreal Engine, targeting a specific engine version and platforms.
  /// </summary>
  /// <param name="descriptorFile">The path to the descriptor file (.uplugin or .uproject) containing the requirements to be installed.</param>
  /// <param name="engineVersion">The Unreal Engine version to target for the installation, or null to use the default engine version.</param>
  /// <param name="platforms">The collection of platforms for which the requirements should be installed.</param>
  /// <returns>A task that represents the asynchronous installation operation. The task result contains the installation result.</returns>
  public Task<List<VersionChange>> InstallRequirements(string descriptorFile, string? engineVersion,
                                                       IReadOnlyCollection<string> platforms);

  /// <summary>
  /// Installs the required plugins specified in the provided manifest for a target Unreal Engine version and platforms.
  /// </summary>
  /// <param name="manifest">The manifest containing the plugin information and dependencies to install.</param>
  /// <param name="engineVersion">The Unreal Engine version for which the plugins should be installed, or null to use the default engine version.</param>
  /// <param name="platforms">The collection of target platforms for the plugin installation.</param>
  /// <returns>A task that represents the asynchronous installation operation. The task result contains a list of version changes applied during the installation.</returns>
  public Task<List<VersionChange>> InstallRequirements(PluginManifest manifest, string? engineVersion,
                                                       IReadOnlyCollection<string> platforms);
}