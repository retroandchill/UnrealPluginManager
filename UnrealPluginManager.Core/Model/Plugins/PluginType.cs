using System.Text.Json.Serialization;

namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Specifies the type of plugin in the Unreal Plugin Manager system.
/// </summary>
/// <remarks>
/// It is utilized in various parts of the Unreal Plugin Manager system, including
/// dependency descriptions and plugin reference mappings.
/// </remarks>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PluginType {
    /// <summary>
    /// Represents plugins that are integral to the engine.
    /// </summary>
    Engine,
    
    /// <summary>
    /// Refers to plugins that are distributed along with a project or framework.
    /// </summary>
    Provided,
    
    /// <summary>
    /// Represents third-party or user-added plugins.
    /// </summary>
    External
}