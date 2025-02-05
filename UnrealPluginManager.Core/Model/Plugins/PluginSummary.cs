using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Semver;
using UnrealPluginManager.Core.Converters;

namespace UnrealPluginManager.Core.Model.Plugins;

public class PluginSummary(string name, SemVersion version, string? description) {

    [Required]
    public string Name { get; } = name;
    
    [Required]
    [JsonConverter(typeof(SemVersionJsonConverter))]
    public SemVersion Version { get; } = version;
    
    public string? Description { get; } = description;

}