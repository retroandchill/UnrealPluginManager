using UnrealPluginManager.Core.Model.Plugins;
using LanguageExt;

namespace UnrealPluginManager.Cli.Services;

/// <summary>
/// Represents a service used for remote calls to fetch plugin data.
/// </summary>
public interface IRemoteCallService {

    /// <summary>
    /// Retrieves a list of plugin overviews that optionally match a given search term.
    /// The method aggregates and groups plugins by name, merging relevant details
    /// from different sources if applicable.
    /// </summary>
    /// <param name="searchTerm">
    /// An optional string used to filter plugins by matching their names or other attributes.
    /// If null or empty, all available plugins are returned.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. Upon completion, returns an
    /// <see cref="OrderedDictionary"/> where each key is a plugin identifier and each value
    /// is a <see cref="Fin{T}"/> containing a list of <see cref="PluginOverview"/> objects.
    /// </returns>
    Task<OrderedDictionary<string, Fin<List<PluginOverview>>>> GetPlugins(string searchTerm);

    /// <summary>
    /// Retrieves a list of plugin overviews from the specified remote source that optionally match a given search term.
    /// The method queries the remote API and fetches relevant plugin details filtered by the search term if provided.
    /// </summary>
    /// <param name="remote">
    /// A string representing the identifier of the remote source where the plugins are fetched from.
    /// This value determines the specific remote API to query for plugins.
    /// </param>
    /// <param name="searchTerm">
    /// An optional string used to filter plugins by matching their names or other attributes.
    /// If null or empty, all available plugins from the specified remote source are returned.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation. Upon completion, returns a
    /// list of <see cref="PluginOverview"/> objects containing detailed information about the plugins retrieved.
    /// </returns>
    Task<List<PluginOverview>> GetPlugins(string remote, string searchTerm);

    /// <summary>
    /// Resolves remote dependencies for a given list of root plugin dependencies.
    /// The method attempts to find compatible versions of the specified plugins by consulting remote sources,
    /// extending the provided local dependency manifest with additional resolved or unresolved entries.
    /// </summary>
    /// <param name="rootDependencies">
    /// A list of root plugin dependencies that need to be resolved.
    /// Each dependency represents a specific plugin and its requirements.
    /// </param>
    /// <param name="localManifest">
    /// An existing dependency manifest containing previously resolved and unresolved dependencies.
    /// This manifest is extended with the results from the remote resolution process.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation. Upon completion, returns a <see cref="DependencyManifest"/>
    /// which includes a dictionary of resolved dependencies along with their versions
    /// and a set of unresolved dependency names.
    /// </returns>
    Task<DependencyManifest> TryResolveRemoteDependencies(List<PluginDependency> rootDependencies,
        DependencyManifest localManifest);
    
    

}