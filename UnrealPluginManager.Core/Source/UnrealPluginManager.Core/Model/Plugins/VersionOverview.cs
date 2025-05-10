using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Semver;
using UnrealPluginManager.Core.Converters;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Core.Model.Storage;

namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents a specific version of a plugin, identified by a unique ID and version number.
/// </summary>
public class VersionOverview {
  /// <summary>
  /// Gets or sets the unique identifier for this instance.
  /// </summary>
  [Required]
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the semantic version of the plugin, representing its specific version details.
  /// </summary>
  [Required]
  [JsonConverter(typeof(SemVersionJsonConverter))]
  public required SemVersion Version { get; set; }

  public SourceLocation Source { get; set; }


  /// <summary>
  /// Gets or sets the icon resource associated with a version of a plugin.
  /// </summary>
  public ResourceInfo? Icon { get; set; }
}