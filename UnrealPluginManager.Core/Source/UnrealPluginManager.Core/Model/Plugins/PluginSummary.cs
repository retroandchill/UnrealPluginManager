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
  public required Guid PluginId { get; init; }

  /// <summary>
  /// Gets the name of the plugin.
  /// This property is required and uniquely identifies the plugin within the context of the plugin system.
  /// </summary>
  [Required]
  public required string Name { get; init; }


  /// <summary>
  /// Gets the unique identifier of the plugin version.
  /// This property ensures that each version of a plugin is distinctly identifiable within the system.
  /// </summary>
  [Required]
  [Range(1, long.MaxValue)]
  public required Guid VersionId { get; init; }

  /// <summary>
  /// Gets the semantic version of the plugin.
  /// This property adheres to the semantic versioning format and is used to specify the version details of the plugin.
  /// </summary>
  [Required]
  [JsonConverter(typeof(SemVersionJsonConverter))]
  public required SemVersion Version { get; init; }

  /// <summary>
  /// Indicates whether the plugin version is currently installed.
  /// </summary>
  /// <remarks>
  /// This property specifies the installation status of the plugin version. It can be
  /// used to determine if a plugin version is available on the system for use or
  /// requires installation. The value `true` represents that the plugin is installed,
  /// while `false` indicates it is not installed.
  /// </remarks>
  [JsonIgnore]
  public bool Installed { get; set; }


  /// <summary>
  /// Gets or sets the index of the plugin version as retrieved from a remote source.
  /// </summary>
  /// <remarks>
  /// This property is used to track the position or order of the plugin version
  /// in a dataset originating from an external or remote repository. It is
  /// primarily utilized for managing or querying the plugin's metadata
  /// in scenarios involving remote data synchronization or retrieval.
  /// </remarks>
  [JsonIgnore]
  public int? RemoteIndex { get; set; }
}