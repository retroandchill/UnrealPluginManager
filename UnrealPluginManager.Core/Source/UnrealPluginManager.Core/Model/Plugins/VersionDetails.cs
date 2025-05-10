namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents detailed information about a specific plugin version,
/// including its version number, unique identifier, and list of associated binaries.
/// </summary>
public class VersionDetails : VersionOverview {
  /// <summary>
  /// A collection of dependency overviews associated with a specific version of a plugin.
  /// Each dependency overview provides information about dependent plugins, including their metadata, version requirements, and optionality.
  /// </summary>
  public required List<DependencyOverview> Dependencies { get; set; }
}