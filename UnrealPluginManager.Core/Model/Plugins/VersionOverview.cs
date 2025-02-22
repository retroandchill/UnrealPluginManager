using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Semver;
using UnrealPluginManager.Core.Converters;

namespace UnrealPluginManager.Core.Model.Plugins;

#nullable disable

/// <summary>
/// Represents a specific version of a plugin, identified by a unique ID and version number.
/// </summary>
public class VersionOverview {

    /// <summary>
    /// Gets or sets the unique identifier for this instance.
    /// </summary>
    [Required]
    public ulong Id { get; set; }

    /// <summary>
    /// Gets or sets the semantic version of the plugin, representing its specific version details.
    /// </summary>
    [Required]
    [JsonConverter(typeof(SemVersionJsonConverter))]
    public SemVersion Version { get; set; }
    
}