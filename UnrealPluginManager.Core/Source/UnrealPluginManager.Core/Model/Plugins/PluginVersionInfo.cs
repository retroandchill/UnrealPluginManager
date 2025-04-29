using System.Text.Json.Serialization;
using Semver;
using UnrealPluginManager.Core.Converters;
using UnrealPluginManager.Core.Model.Storage;

namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents detailed information about a specific version of a plugin.
/// </summary>
/// <remarks>
/// This class provides properties to store the plugin's name, its version number,
/// and a list of its dependencies. It is used to model the metadata and relationships
/// of a plugin version within the system.
/// </remarks>
public class PluginVersionInfo : IDependencyChainNode {
  /// <summary>
  /// Gets or sets the unique identifier for a plugin.
  /// </summary>
  /// <remarks>
  /// The identifier is used to represent a specific plugin across various operations
  /// and mappings within the plugin management system. This value must be consistent
  /// for the plugin it represents and is essential for ensuring reliable identification
  /// across components interacting with the plugin.
  /// </remarks>
  public required Guid PluginId { get; set; }

  /// <summary>
  /// Gets the name of the plugin associated with the current version.
  /// </summary>
  /// <remarks>
  /// The plugin name is a unique identifier for the plugin and is generally used
  /// to distinguish it from other plugins. It follows specific validation rules, such as
  /// starting with an uppercase letter and containing only alphanumeric characters.
  /// </remarks>
  public required string Name { get; set; }

  /// <summary>
  /// Gets or sets the user-friendly name of the plugin associated with the current version.
  /// </summary>
  /// <remarks>
  /// The friendly name is a descriptive and more readable representation of the plugin's identity
  /// intended to be displayed to end-users. It may contain spaces and other characters,
  /// unlike the unique plugin name used internally.
  /// </remarks>
  public string? FriendlyName { get; set; }

  /// <summary>
  /// Gets or sets a brief explanation or summary of the plugin version.
  /// This provides additional context or details about the plugin functionality or purpose.
  /// </summary>
  public string? Description { get; set; }

  /// <summary>
  /// Gets or sets the name of the author associated with the plugin.
  /// </summary>
  public string? AuthorName { get; set; }


  /// <summary>
  /// Gets or sets the URL associated with the author of the plugin.
  /// </summary>
  /// <remarks>
  /// This property contains a link to additional information about the plugin's author,
  /// such as a personal website, portfolio, or profile page. It provides users with
  /// a way to learn more about the author or contact them if necessary. The value is
  /// optional and may be null if no URL is provided.
  /// </remarks>
  public string? AuthorWebsite { get; set; }

  /// <summary>
  /// Gets or sets the unique identifier for the plugin version.
  /// </summary>
  /// <remarks>
  /// The VersionId is a numerical value that uniquely identifies a specific version
  /// of the plugin. It is used to track and manage individual plugin versions
  /// and their associations with other plugin metadata.
  /// </remarks>
  public required Guid VersionId { get; set; }

  /// <summary>
  /// Gets the semantic version of the plugin.
  /// </summary>
  /// <remarks>
  /// The version follows semantic versioning conventions, represented by major, minor, and patch numbers.
  /// It is used to determine compatibility and precedence among plugin versions.
  /// </remarks>
  [JsonConverter(typeof(SemVersionJsonConverter))]
  public required SemVersion Version { get; set; }

  /// <summary>
  /// Gets or sets the resource information for the plugin icon.
  /// </summary>
  /// <remarks>
  /// This property holds the metadata or path information related to the plugin's icon.
  /// It is used to define and manage visual representation for the plugin within the system.
  /// The icon is optional and can be null if no specific icon is associated with the plugin.
  /// </remarks>
  public ResourceInfo? Icon { get; set; }

  /// <summary>
  /// Gets or sets the list of dependencies for the current plugin version.
  /// </summary>
  /// <remarks>
  /// This property represents the collection of plugins that the current plugin
  /// version depends on. Each dependency includes details such as the plugin's name,
  /// type, and its compatible version range.
  /// </remarks>
  public required List<PluginDependency> Dependencies { get; set; }

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