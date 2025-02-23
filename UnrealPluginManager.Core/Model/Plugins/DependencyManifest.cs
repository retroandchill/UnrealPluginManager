namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents a manifest of plugin dependencies resolved by the dependency resolution process.
/// </summary>
/// <remarks>
/// This class is typically used to store information related to resolved and unresolved dependencies
/// for plugins within the UnrealPluginManager system.
/// </remarks>
public class DependencyManifest {
    /// <summary>
    /// A property that stores a collection of dependencies that have been successfully resolved
    /// during the dependency resolution process.
    /// </summary>
    /// <remarks>
    /// The property is a dictionary where the keys represent the names of dependencies as strings,
    /// and the values are lists of <see cref="PluginVersionInfo"/> objects, which provide detailed
    /// versioning and metadata information for each resolved dependency.
    /// </remarks>
    public Dictionary<string, List<PluginVersionInfo>> FoundDependencies { get; set; } = [];

    /// <summary>
    /// A property that holds a collection of dependencies that could not be resolved
    /// during the dependency resolution process.
    /// </summary>
    /// <remarks>
    /// This property is a list of strings where each string represents the name
    /// of a plugin dependency that remains unresolved. It is typically updated
    /// during the execution of the dependency resolution workflow to track and
    /// manage unresolved dependencies for plugins.
    /// </remarks>
    public List<string> UnresolvedDependencies { get; set; } = [];

}