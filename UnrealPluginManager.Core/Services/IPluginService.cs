using LanguageExt;
using Semver;
using UnrealPluginManager.Core.Model.Engine;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Pagination;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Service declaration for operations involving plugins.
/// </summary>
public interface IPluginService {
    /// <summary>
    /// Retrieves a collection of plugin summaries including essential information like name and optional description.
    /// </summary>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <returns>
    /// An enumerable collection of <see cref="PluginSummary"/> representing the summaries of all plugins.
    /// </returns>
    Task<Page<PluginSummary>> GetPluginSummaries(int pageNumber, int pageSize);

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
    /// <param name="storedFile">The information about the stored file for the plugin</param>
    /// <returns>
    /// A <see cref="PluginSummary"/> representing the added plugin, including its name and optional description.
    /// </returns>
    Task<PluginSummary> AddPlugin(string pluginName, PluginDescriptor descriptor, EngineFileData? storedFile = null);

    /// <summary>
    /// Submits a plugin file for processing and storage, associating it with a specific engine version.
    /// </summary>
    /// <param name="fileData">
    /// The stream containing the plugin file data to be submitted.
    /// </param>
    /// <param name="engineVersion">
    /// The version of the Unreal Engine that the plugin is associated with.
    /// </param>
    /// <returns>
    /// A <see cref="PluginSummary"/> representing the processed and stored plugin, including its metadata.
    /// </returns>
    Task<PluginSummary> SubmitPlugin(Stream fileData, Version engineVersion);

    /// <summary>
    /// Retrieves the file data of a specific plugin for a given engine version.
    /// </summary>
    /// <param name="pluginName">
    ///     The name of the plugin whose file data is to be retrieved.
    /// </param>
    /// <param name="targetVersion"></param>
    /// <param name="engineVersion">
    ///     The version of the engine for which the plugin file data is needed.
    /// </param>
    /// <returns>
    /// A stream representing the binary content of the plugin file.
    /// </returns>
    Task<Stream> GetPluginFileData(string pluginName, SemVersionRange targetVersion, Version engineVersion);
}