using Semver;

namespace UnrealPluginManager.Core.Model.Resolution;

/// <summary>
/// Represents a requirement for a plugin, specifying the source of the requirement
/// and the version range that satisfies it.
/// </summary>
public record struct PluginRequirement(string RequiredBy, SemVersionRange RequiredVersion);

/// <summary>
/// Represents a conflict that occurs when multiple requirements specify incompatible
/// versions for the same plugin during resolution.
/// </summary>
/// <remarks>
/// A conflict encapsulates the name of the plugin in question, along with a list of
/// specific version requirements that caused the conflict. This allows identifying
/// and diagnosing dependency resolution issues.
/// </remarks>
public record struct Conflict(string PluginName, List<PluginRequirement> Versions);