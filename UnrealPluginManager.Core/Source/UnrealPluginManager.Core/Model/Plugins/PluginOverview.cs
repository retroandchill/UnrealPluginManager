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
  /// Gets or sets the user-friendly name of the plugin, which may be used for display purposes.
  /// </summary>
  public string? FriendlyName { get; set; }

  /// <summary>
  /// Gets or sets the description of the plugin, providing detailed information about its purpose or functionality.
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Gets or sets the name of the author associated with the plugin.
  /// </summary>
  public string? AuthorName { get; set; }

  /// <summary>
  /// Gets or sets the URL associated with the author of the plugin.
  /// </summary>
  public Uri? AuthorWebsite { get; set; }

  /// <summary>
  /// Gets or sets the collection of versions associated with the plugin.
  /// Each version provides a detailed overview including its version number
  /// and unique identifier.
  /// </summary>
  [Required]
  [MinLength(1)]
  public required List<VersionOverview> Versions { get; set; }
}