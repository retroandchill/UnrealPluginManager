using Dunet;
using Semver;
using UnrealPluginManager.Core.Model.Resolution;

namespace UnrealPluginManager.Local.Model.Installation;

/// <summary>
/// Represents a version change for a plugin. This is used to capture the old and new versions
/// of a plugin when an update or installation occurs, as well as the name of the plugin being changed.
/// </summary>
public record struct VersionChange(string PluginName, SemVersion? OldVersion, SemVersion NewVersion);

/// <summary>
/// Represents the result of an installation attempt for a plugin or set of plugins.
/// It can encapsulate various outcomes of the installation process, including successful changes,
/// already installed plugins, or conflicts that prevent installation.
/// </summary>
[Union]
public partial record InstallResult {
  /// <summary>
  /// Represents changes made during the installation process. This includes
  /// a list of version changes which detail the old and new versions of plugins
  /// being updated or installed.
  /// </summary>
  public partial record InstallChanges(List<VersionChange> VersionChanges);

  /// <summary>
  /// Represents an installation result where the plugin or plugins are already installed
  /// and no changes are required. This indicates that the requested installation action
  /// was skipped because the target version is already in place.
  /// </summary>
  public partial record AlreadyInstalled;

  /// <summary>
  /// Represents a record indicating the presence of conflicts during a plugin installation process.
  /// This is used to capture a list of conflicts that prevent the installation or update of plugins.
  /// </summary>
  public partial record HasConflicts(List<Conflict> Conflicts);

  /// <summary>
  /// A static property that represents an installation result where no changes
  /// were needed because the plugin is already installed in the desired state.
  /// </summary>
  public static InstallResult NoChanges { get; } = new AlreadyInstalled();

  /// <summary>
  /// Defines an implicit conversion operator for InstallResult from a list of VersionChange objects.
  /// This conversion creates an InstallChanges result based on the provided version changes.
  /// </summary>
  /// <param name="versionChanges">A list of VersionChange objects representing the plugin version changes applied during the installation process.</param>
  /// <returns>An InstallResult instance of type InstallChanges encapsulating the provided version changes.</returns>
  public static implicit operator InstallResult(List<VersionChange> versionChanges) {
    return new InstallChanges(versionChanges);
  }

  /// <summary>
  /// Defines an implicit conversion operator for InstallResult from a list of Conflict objects.
  /// This conversion creates a HasConflicts result based on the provided conflicts.
  /// </summary>
  /// <param name="conflicts">A list of Conflict objects representing the conflicts encountered during the installation process.</param>
  /// <returns>An InstallResult instance of type HasConflicts encapsulating the provided conflicts.</returns>
  public static implicit operator InstallResult(List<Conflict> conflicts) {
    return new HasConflicts(conflicts);
  }
};