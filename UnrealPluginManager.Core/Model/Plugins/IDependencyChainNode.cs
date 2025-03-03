using Semver;

namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Provides information about a plugin version, including its name, version number, and dependencies.
/// </summary>
public interface IDependencyChainNode {
  /// <summary>
  /// Gets the name of the plugin associated with the current version.
  /// </summary>
  /// <remarks>
  /// The plugin name is a unique identifier for the plugin and is generally used
  /// to distinguish it from other plugins. It follows specific validation rules, such as
  /// starting with an uppercase letter and containing only alphanumeric characters.
  /// </remarks>
  string Name { get; }

  /// <summary>
  /// Gets the semantic version of the plugin.
  /// </summary>
  /// <remarks>
  /// The version follows semantic versioning conventions, represented by major, minor, and patch numbers.
  /// It is used to determine compatibility and precedence among plugin versions.
  /// </remarks>
  SemVersion Version { get; }

  /// <summary>
  /// Gets the collection of dependencies required by the plugin.
  /// </summary>
  /// <remarks>
  /// Each dependency represents another plugin that the current plugin relies upon. These dependencies include
  /// the names, types, and compatible version ranges of the required plugins. This property is primarily used
  /// to resolve and ensure that all necessary plugins are present and meet the specified compatibility requirements.
  /// </remarks>
  List<PluginDependency> Dependencies { get; }

  /// <summary>
  /// Indicates whether the plugin has been installed on the system.
  /// </summary>
  /// <remarks>
  /// This property reflects the installation status of a plugin or dependency.
  /// It is utilized to determine if a plugin is currently available locally or
  /// needs to be retrieved through other means, such as downloading from a remote source.
  /// </remarks>
  public bool Installed { get; }

  /// <summary>
  /// Gets or sets the index of the plugin in the remote repository or database.
  /// </summary>
  /// <remarks>
  /// The RemoteIndex indicates the position or identifier of the plugin in a remote structure,
  /// such as a repository or service listing. This property can be null if the plugin does not
  /// have a corresponding entry in the remote system.
  /// </remarks>
  public int? RemoteIndex { get; }
}