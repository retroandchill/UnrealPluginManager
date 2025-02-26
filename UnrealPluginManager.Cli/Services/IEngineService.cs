using System.IO.Abstractions;
using Semver;
using UnrealPluginManager.Cli.Model.Engine;
using UnrealPluginManager.Core.Model.EngineFile;
using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Cli.Services;

/// <summary>
/// Provides functionalities related to engine management within the Unreal Plugin Manager CLI.
/// </summary>
public interface IEngineService {
    /// <summary>
    /// Reads the specified plugin file and returns a collection of plugin dependencies.
    /// </summary>
    /// <param name="filename">The path to the plugin file to be read.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains an <see cref="IDependencyHolder"/> object holding the plugin dependencies.</returns>
    public Task<IDependencyHolder> ReadSubmittedPluginFile(string filename);
    
    /// <summary>
    /// Retrieves a list of installed Unreal Engine versions available on the system.
    /// </summary>
    /// <returns>
    /// A list of <see cref="InstalledEngine"/> objects representing the installed Unreal Engine versions.
    /// </returns>
    List<InstalledEngine> GetInstalledEngines();

    /// <summary>
    /// Builds a specified Unreal Engine plugin using the provided engine version.
    /// </summary>
    /// <param name="pluginFile">
    ///     The <see cref="IFileInfo"/> representing the path of the plugin file to be built.
    /// </param>
    /// <param name="engineVersion">
    ///     The version of the Unreal Engine to use for building the plugin. If null, the default engine version is used.
    /// </param>
    /// <returns>
    /// A task representing the asynchronous operation with an integer exit code indicating the result of the build process.
    /// </returns>
    public Task<int> BuildPlugin(IFileInfo pluginFile, string? engineVersion);

    /// <summary>
    /// Installs the specified plugin for a given Unreal Engine version.
    /// </summary>
    /// <param name="pluginName">The name of the plugin to install.</param>
    /// <param name="pluginVersion">The version of the plugin to install, specified as a semantic version range.</param>
    /// <param name="engineVersion">The target Unreal Engine version for the installation, or null to use the default.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains an integer indicating the status of the installation process.
    /// </returns>
    public Task<int> InstallPlugin(string pluginName, SemVersionRange pluginVersion, string? engineVersion);

    /// <summary>
    /// Retrieves a list of installed plugins for a specified Unreal Engine version.
    /// </summary>
    /// <param name="engineVersion">The version of the Unreal Engine for which to list installed plugins. If null, lists plugins for all available engine versions.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a list of <see cref="PluginVersionRequest"/> objects representing the installed plugins.</returns>
    public Task<List<PluginVersionInfo>> GetInstalledPluginVersions(string? engineVersion = null);
    
    /// <summary>
    /// Retrieves a list of installed plugin versions for the specified plugin names within the specified engine version.
    /// </summary>
    /// <param name="pluginNames">
    ///     A list of plugin names to filter the installed plugin versions.
    /// </param>
    /// <param name="engineVersion">
    ///     The version of the engine to look for installed plugins. If null, uses the default engine version.
    /// </param>
    /// <returns>
    /// A list of <see cref="PluginVersionInfo"/> objects representing the details of installed plugin versions.
    /// </returns>
    public Task<List<PluginVersionInfo>> GetInstalledPluginVersions(IEnumerable<string> pluginNames,
        string? engineVersion);
}