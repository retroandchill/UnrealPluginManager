using System.IO.Abstractions;

namespace UnrealPluginManager.Local.Model.Engine;

/// <summary>
/// Represents an installed Unreal Engine instance, encapsulating its name,
/// version, directory path, and whether it is a custom build.
/// </summary>
/// <remarks>
/// This record provides utility properties for accessing key directories
/// within the engine's folder structure and constructing descriptive names
/// for display purposes.
/// </remarks>
/// <param name="Name">
/// The name of the installed engine. Typically corresponds to the user-defined
/// or system-defined identifier for this engine instance.
/// </param>
/// <param name="Version">
/// The version of the engine. This reflects the specific Unreal Engine version,
/// such as 4.27, 5.0, etc.
/// </param>
/// <param name="Directory">
/// The root directory of the installed engine instance.
/// </param>
/// <param name="CustomBuild">
/// Indicates whether the engine is a custom build or an official Epic Games release.
/// The default value is false for official releases.
/// </param>
public record InstalledEngine(string Name, Version Version, IDirectoryInfo Directory, bool CustomBuild = false) {
    /// <summary>
    /// Gets a string that represents the display name of the installed engine.
    /// The display name includes additional information to indicate whether the
    /// engine is a custom build or an installed version. If the engine is a custom build,
    /// the name is used with a "Custom Build" suffix. Otherwise, it uses the version
    /// with an "Installed" suffix.
    /// </summary>
    public string DisplayName => CustomBuild ? $"{Name}: Custom Build" : $"{Version}: Installed";

    /// <summary>
    /// Gets the full directory path to the "Engine" folder within the installed Unreal Engine instance.
    /// This folder contains essential engine components and subdirectories
    /// used for development and runtime purposes.
    /// </summary>
    public string EngineDirectory => Path.Join(Directory.FullName, "Engine");

    /// <summary>
    /// Gets the directory path containing batch files related to building and managing the engine.
    /// This directory is typically located within the engine's build folder and includes scripts
    /// used for various automation tasks.
    /// </summary>
    public string BatchFilesDirectory => Path.Join(EngineDirectory, "Build", "BatchFiles");

    /// <summary>
    /// Gets the directory path where engine plugins are located within the installed Unreal Engine instance.
    /// This directory serves as the default location for storing and managing plugins.
    /// </summary>
    public string PluginDirectory => Path.Join(EngineDirectory, "Plugins");

    /// <summary>
    /// Gets the directory path where marketplace plugins are located within the engine's folder structure.
    /// The marketplace directory typically contains plugins that are either downloaded
    /// or installed from external sources, including the Unreal Engine Marketplace.
    /// </summary>
    public string MarketplaceDirectory => Path.Join(PluginDirectory, "Marketplace");

    /// <summary>
    /// Gets the file path to the package directory used by the Unreal Plugin Manager.
    /// This directory resides in the Marketplace plugin directory of the installed engine
    /// and contains plugin-related data specific to Unreal Plugin Manager.
    /// </summary>
    public string PackageDirectory => Path.Join(MarketplaceDirectory, ".UnrealPluginManager");

}