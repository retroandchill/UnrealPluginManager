using System.Text.Json.Serialization;
using Semver;
using UnrealPluginManager.Core.Converters;

namespace UnrealPluginManager.Core.Model.Plugins.Recipes;

/// <summary>
/// Represents the dependency manifest of a plugin, detailing the required plugin dependencies.
/// It includes information about the dependency's name, version constraints, and optional repository source.
/// </summary>
public record PluginDependencyManifest {
  /// <summary>
  /// Gets or sets the name of the plugin dependency.
  /// </summary>
  public required string Name { get; init; }

  /// <summary>
  /// Gets or sets the version constraint of the plugin dependency.
  /// Defines the acceptable range of versions required for the dependency.
  /// </summary>
  [JsonConverter(typeof(SemVersionRangeJsonConverter))]
  public SemVersionRange Version { get; init; } = SemVersionRange.AllRelease;
}