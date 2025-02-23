using Semver;

namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents a dependency of a plugin within the Unreal Plugin Manager framework.
/// </summary>
/// <remarks>
/// A plugin dependency contains the name of the dependent plugin, its type, and a version specification
/// indicating the compatible versions of the plugin.
/// </remarks>
public class PluginDependency : IPluginDependency {
    
    /// <inheritdoc />
    public required string PluginName { get; set; }
    
    /// <inheritdoc />
    public required PluginType Type { get; set; }
    
    /// <inheritdoc />
    public required SemVersionRange PluginVersion { get; set; }
}