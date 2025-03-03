using System.ComponentModel.DataAnnotations;
using Semver;

namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents an overview of a plugin, providing basic information such as its ID, name,
/// optional friendly name, description, and associated versions.
/// </summary>
public class PluginOverview : PluginOverviewBase {
  /// <summary>
  /// Gets or sets the collection of versions associated with the plugin.
  /// Each version provides a detailed overview including its version number
  /// and unique identifier.
  /// </summary>
  [Required]
  [MinLength(1)]
  public required List<VersionOverview> Versions { get; set; }
}