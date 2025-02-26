using Semver;

namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents a request for a specific plugin's version details.
/// </summary>
/// <remarks>
/// This record struct encapsulates a plugin's name along with its version.
/// It is used in scenarios where version information for specific plugins is queried, such as retrieving metadata or compatibility details.
/// </remarks>
/// <param name="Name">The name of the plugin for which the version information is requested.</param>
/// <param name="Version">The specific version of the plugin being requested.</param>
public record struct PluginVersionRequest(string Name, SemVersion Version);