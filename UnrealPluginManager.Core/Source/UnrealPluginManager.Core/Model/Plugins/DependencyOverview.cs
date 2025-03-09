using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Semver;
using UnrealPluginManager.Core.Converters;

namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents an overview of a plugin dependency, including its metadata and type.
/// </summary>
public class DependencyOverview {
  /// <summary>
  /// Gets or sets the unique identifier associated with a dependency.
  /// This identifier is automatically generated and used to distinguish
  /// dependencies within the system.
  /// </summary>
  [Required]
  [Range(1, long.MaxValue)]
  public long Id { get; set; }

  /// <summary>
  /// Gets or sets the name of the plugin associated with the dependency.
  /// The plugin name is a required field and must adhere to the predefined naming rules.
  /// </summary>
  public required string PluginName { get; set; }

  /// <summary>
  /// Gets or sets the version range associated with the plugin dependency.
  /// This version range defines the compatible versions of the plugin required by the dependency.
  /// </summary>
  [JsonConverter(typeof(SemVersionRangeJsonConverter))]
  public SemVersionRange PluginVersion { get; set; } = SemVersionRange.All;

  /// <summary>
  /// Gets or sets a value indicating whether the dependency is optional.
  /// When set to true, the dependency is not mandatory, and the system
  /// can function without it.
  /// </summary>
  public bool Optional { get; set; }

  /// <summary>
  /// Gets or sets the type of plugin associated with this dependency.
  /// Specifies whether the dependency is an Engine plugin, a Provided plugin,
  /// or an External plugin.
  /// </summary>
  public PluginType Type { get; set; } = PluginType.Provided;
}