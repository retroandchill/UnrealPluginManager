using System.Text.Json.Serialization;
using Semver;

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
    public required ulong PluginId { get; set; }
    
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
    /// Gets or sets the unique identifier for the plugin version.
    /// </summary>
    /// <remarks>
    /// The VersionId is a numerical value that uniquely identifies a specific version
    /// of the plugin. It is used to track and manage individual plugin versions
    /// and their associations with other plugin metadata.
    /// </remarks>
    public required ulong VersionId { get; set; }
    
    /// <summary>
    /// Gets the semantic version of the plugin.
    /// </summary>
    /// <remarks>
    /// The version follows semantic versioning conventions, represented by major, minor, and patch numbers.
    /// It is used to determine compatibility and precedence among plugin versions.
    /// </remarks>
    public required SemVersion Version { get; set; }

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
    public bool IsInstalled { get; set; }

    /// <summary>
    /// Gets or sets the name of the plugin as retrieved from a remote source or repository.
    /// </summary>
    /// <remarks>
    /// This property represents the name of the plugin as it is defined in a remote system. It may differ
    /// from its local or friendly name and is often used for referencing or synchronization purposes when
    /// interacting with external systems.
    /// </remarks>
    [JsonIgnore]
    public string? RemoteName { get; set; }
}