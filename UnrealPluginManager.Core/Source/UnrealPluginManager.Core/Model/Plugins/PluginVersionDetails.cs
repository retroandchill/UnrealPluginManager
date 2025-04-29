namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents detailed information about a specific plugin version,
/// including a description and an overview of its binaries.
/// </summary>
/// <remarks>
/// Inherits from <see cref="PluginVersionInfo"/> and adds additional details about the plugin version.
/// This includes properties for an optional description and a list of binaries associated with the plugin version.
/// </remarks>
public class PluginVersionDetails : PluginVersionInfo {
  /// <summary>
  /// Gets or sets the collection of binaries associated with the plugin version.
  /// This provides details about the compiled binaries, such as their configurations or associated metadata.
  /// </summary>
  public List<BinariesOverview> Binaries { get; set; } = [];

}