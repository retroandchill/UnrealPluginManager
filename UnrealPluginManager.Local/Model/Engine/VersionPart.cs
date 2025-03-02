namespace UnrealPluginManager.Local.Model.Engine;

/// <summary>
/// Represents the components of a version number.
/// </summary>
/// <remarks>
/// A version is typically composed of multiple parts, such as major, minor,
/// and patch numbers. This enum is used to specify which part of a version is
/// being referenced or manipulated.
/// </remarks>
public enum VersionPart {
    /// <summary>
    /// The major version.
    /// </summary>
    Major,
    
    /// <summary>
    /// The minor version.
    /// </summary>
    Minor,
    
    /// <summary>
    /// The patch version
    /// </summary>
    Patch
}