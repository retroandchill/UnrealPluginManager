using System.ComponentModel.DataAnnotations;

namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents detailed information about a plugin, including its associated versions.
/// Inherits from <see cref="PluginOverviewBase"/> to provide core metadata.
/// </summary>
public class PluginDetails : PluginOverviewBase {
  /// <summary>
  /// Gets or sets the collection of versions associated with the plugin.
  /// Each version provides a detailed overview including its version number
  /// and unique identifier.
  /// </summary>
  [Required]
  [MinLength(1)]
  public required List<VersionDetails> Versions { get; set; }
}