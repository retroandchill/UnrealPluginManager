using System.ComponentModel.DataAnnotations;

namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Provides a base class for representing essential metadata of a plugin.
/// This includes core properties such as the plugin's unique identifier, name,
/// optional friendly name, description, author's name, and website.
/// </summary>
public abstract class PluginOverviewBase {
  /// <summary>
  /// Gets or sets the unique identifier for the plugin.
  /// </summary>

  [Required]
  [Range(1, long.MaxValue)]
  public required long Id { get; set; }

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
}