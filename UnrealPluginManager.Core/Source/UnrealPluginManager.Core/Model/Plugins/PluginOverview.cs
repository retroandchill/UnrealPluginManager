using System.ComponentModel.DataAnnotations;

namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents an overview of a plugin, providing basic information such as its ID, name,
/// optional friendly name, description, and associated versions.
/// </summary>
public class PluginOverview {
  /// <summary>
  /// Gets or sets the unique identifier for the plugin.
  /// </summary>

  [Required]
  public required Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the name of the plugin.
  /// </summary>
  [Required]
  public required string Name { get; set; }

  /// <summary>
  /// Gets or sets the collection of versions associated with the plugin.
  /// Each version provides a detailed overview including its version number
  /// and unique identifier.
  /// </summary>
  [Required]
  [MinLength(1)]
  public required List<VersionOverview> Versions { get; set; }
}