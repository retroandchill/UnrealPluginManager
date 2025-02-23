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
public class PluginVersionInfo : IPluginVersionInfo {
    /// <inheritdoc />
    public required string PluginName { get; set; }
    
    
    /// <inheritdoc />
    public required SemVersion Version { get; set; }

    IEnumerable<IPluginDependency> IPluginVersionInfo.Dependencies => Dependencies;


    /// <summary>
    /// Gets or sets the list of dependencies for the current plugin version.
    /// </summary>
    /// <remarks>
    /// This property represents the collection of plugins that the current plugin
    /// version depends on. Each dependency includes details such as the plugin's name,
    /// type, and its compatible version range.
    /// </remarks>
    public required List<PluginDependency> Dependencies { get; set; }
}