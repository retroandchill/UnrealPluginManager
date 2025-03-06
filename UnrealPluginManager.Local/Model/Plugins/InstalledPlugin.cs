using Semver;

namespace UnrealPluginManager.Local.Model.Plugins;

/// <summary>
/// Represents a plugin that has been installed in the Unreal Engine environment.
/// </summary>
/// <remarks>
/// This record struct encapsulates information about a plugin, including its name, version,
/// and the platforms it supports. It is used within the Unreal Plugin Manager to manage
/// installed plugins effectively.
/// </remarks>
/// <param name="Name">
/// The name of the installed plugin. This value is typically derived from the plugin's identifier.
/// </param>
/// <param name="Version">
/// The semantic version of the installed plugin. It uses the SemVersion type to enforce proper versioning semantics.
/// </param>
/// <param name="Platforms">
/// A list of platform names where the plugin is compatible or intended to run.
/// </param>
public record struct InstalledPlugin(string Name, SemVersion Version, List<string> Platforms);