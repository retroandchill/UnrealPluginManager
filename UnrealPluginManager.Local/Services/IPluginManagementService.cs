using LanguageExt;
using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// Represents a service used for remote calls to fetch plugin data.
/// </summary>
public interface IPluginManagementService {
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
  /// <see cref="OrderedDictionary{TKey, TValue}"/> where each key is a plugin identifier and each value
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
  /// Retrieves a list of plugins that need to be installed based on the provided root dependency chain node
  /// and the specified engine version.
  /// </summary>
  /// <param name="root">
  /// The root node of the dependency chain that defines the plugin and its dependencies.
  /// </param>
  /// <param name="engineVersion">
  /// The version of the game engine, which is used to determine compatibility with plugins.
  /// This can be null if engine version filtering is not required.
  /// </param>
  /// <returns>
  /// A task representing the asynchronous operation. Upon completion, returns a <see cref="List{PluginSummary}"/>
  /// containing the plugins that need to be installed, including their details.
  /// </returns>
  Task<List<PluginSummary>> GetPluginsToInstall(IDependencyChainNode root, string? engineVersion);
}