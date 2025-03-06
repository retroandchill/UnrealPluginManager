using Semver;
using InstallResult = UnrealPluginManager.Local.Model.Installation.InstallResult;

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
  public Task<InstallResult> InstallPlugin(string pluginName, SemVersionRange pluginVersion, string? engineVersion, IReadOnlyCollection<string> platforms);

  /// <summary>
  /// Installs the requirements defined in a descriptor file for Unreal Engine, targeting a specific engine version and platforms.
  /// </summary>
  /// <param name="descriptorFile">The path to the descriptor file (.uplugin or .uproject) containing the requirements to be installed.</param>
  /// <param name="engineVersion">The Unreal Engine version to target for the installation, or null to use the default engine version.</param>
  /// <param name="platforms">The collection of platforms for which the requirements should be installed.</param>
  /// <returns>A task that represents the asynchronous installation operation. The task result contains the installation result.</returns>
  public Task<InstallResult> InstallRequirements(string descriptorFile, string? engineVersion, IReadOnlyCollection<string> platforms);
}