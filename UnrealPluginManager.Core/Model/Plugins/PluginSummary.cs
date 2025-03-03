using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Semver;
using UnrealPluginManager.Core.Converters;

namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents a summary of a plugin, including its name, version, and optional description.
/// </summary>
/// <remarks>
/// This class provides essential details about a plugin and serves as a DTO for plugin-related operations.
/// </remarks>
public record PluginSummary {
  /// <summary>
  /// Gets the unique identifier of the plugin.
  /// This property is required and serves as the primary identifier for the plugin within the system.
  /// </summary>
  [Required]
  [Range(1, ulong.MaxValue)]
  public required ulong PluginId { get; init; }

  /// <summary>
  /// Gets the name of the plugin.
  /// This property is required and uniquely identifies the plugin within the context of the plugin system.
  /// </summary>
  [Required]
  public required string Name { get; init; }

  /// <summary>
  /// Gets or sets an optional user-friendly name for the plugin.
  /// This property provides a more descriptive or colloquial name that can be displayed in user interfaces
  /// and used as an alternative to the plugin's primary name when needed.
  /// </summary>
  public required string? FriendlyName { get; init; }


  /// <summary>
  /// Gets the unique identifier of the plugin version.
  /// This property ensures that each version of a plugin is distinctly identifiable within the system.
  /// </summary>
  [Required]
  [Range(1, ulong.MaxValue)]
  public required ulong VersionId { get; init; }

  /// <summary>
  /// Gets the semantic version of the plugin.
  /// This property adheres to the semantic versioning format and is used to specify the version details of the plugin.
  /// </summary>
  [Required]
  [JsonConverter(typeof(SemVersionJsonConverter))]
  public required SemVersion Version { get; init; }
}