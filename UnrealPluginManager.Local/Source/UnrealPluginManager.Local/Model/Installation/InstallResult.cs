using Semver;

namespace UnrealPluginManager.Local.Model.Installation;

/// <summary>
/// Represents a version change for a plugin. This is used to capture the old and new versions
/// of a plugin when an update or installation occurs, as well as the name of the plugin being changed.
/// </summary>
public record struct VersionChange(string PluginName, SemVersion? OldVersion, SemVersion NewVersion);