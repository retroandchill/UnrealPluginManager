using LanguageExt;
using UnrealPluginManager.Core.Model.Engine;
using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Service declaration for operations involving plugins.
/// </summary>
public interface IPluginService {
    /// <summary>
    /// Retrieves a collection of plugin summaries including essential information like name and optional description.
    /// </summary>
    /// <returns>
    /// An enumerable collection of <see cref="PluginSummary"/> representing the summaries of all plugins.
    /// </returns>
    Task<List<PluginSummary>> GetPluginSummaries();

    /// <summary>
    /// Retrieves the list of dependencies for a specified plugin, including both direct and transitive dependencies.
    /// </summary>
    /// <param name="pluginName">
    ///     The name of the plugin for which to retrieve the dependency list.
    /// </param>
    /// <returns>
    /// A collection of <see cref="PluginSummary"/> objects representing the dependencies of the specified plugin.
    /// </returns>
    Task<List<PluginSummary>> GetDependencyList(string pluginName);

    /// <summary>
    /// Adds a new plugin to the system using the provided plugin name and descriptor information.
    /// </summary>
    /// <param name="pluginName">
    ///     The unique name of the plugin to be added.
    /// </param>
    /// <param name="descriptor">
    ///     A <see cref="PluginDescriptor"/> object containing detailed metadata about the plugin, such as version, description, and other attributes.
    /// </param>
    /// <param name="storedFile"></param>
    /// <returns>
    /// A <see cref="PluginSummary"/> representing the added plugin, including its name and optional description.
    /// </returns>
    Task<PluginSummary> AddPlugin(string pluginName, PluginDescriptor descriptor, EngineFileData? storedFile = null);

    Task<PluginSummary> SubmitPlugin(Stream fileData, Version engineVersion);
    
    Task<Stream> GetPluginFileData(string pluginName, Version engineVersion); 
}