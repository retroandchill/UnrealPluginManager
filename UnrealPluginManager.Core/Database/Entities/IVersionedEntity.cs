using Semver;

namespace UnrealPluginManager.Core.Database.Entities;

/// <summary>
/// Represents an entity that has an associated semantic version.
/// This interface is designed to be implemented by classes that
/// require versioning support using the semantic versioning format.
/// </summary>
public interface IVersionedEntity {

    /// <summary>
    /// Gets or sets the major version number of the plugin version.
    /// </summary>
    public int Major { get; }

    /// <summary>
    /// Gets or sets the minor version number of the plugin.
    /// </summary>
    public int Minor { get; }

    /// <summary>
    /// Gets or sets the patch version number of the plugin.
    /// This is a part of the semantic versioning system (major.minor.patch) used to define the plugin version.
    /// </summary>
    public int Patch { get; }

    /// <summary>
    /// Gets or sets the prerelease label of the plugin version.
    /// Represents the prerelease identifier in a semantic version, such as "alpha", "beta", or "rc".
    /// </summary>
    public string? Prerelease { get; }

    /// <summary>
    /// Gets or sets the release candidate number for the plugin version.
    /// This property indicates the pre-release state of the software
    /// and is used in conjunction with semantic versioning.
    /// </summary>
    public int? PrereleaseNumber { get; }

    /// <summary>
    /// Gets or sets the metadata associated with the plugin version.
    /// This typically stores additional version information in a dot-separated format,
    /// which can include build information or other custom descriptors.
    /// </summary>
    public string? Metadata { get; }
    
}