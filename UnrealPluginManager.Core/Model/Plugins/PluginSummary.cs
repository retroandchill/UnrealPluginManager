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
    
    public required ulong PluginId { get; init; }
    
    /// <summary>
    /// Gets the name of the plugin.
    /// This property is required and uniquely identifies the plugin within the context of the plugin system.
    /// </summary>
    [Required]
    public required string Name { get; init; }
    
    public required string? FriendlyName { get; init; }
    
    public required ulong VersionId { get; init; }

    /// <summary>
    /// Gets the version of the plugin.
    /// This property represents the semantic version of the plugin, which adheres to semantic versioning rules.
    /// </summary>
    [Required]
    [JsonConverter(typeof(SemVersionJsonConverter))]
    public required SemVersion Version { get; init; }
}