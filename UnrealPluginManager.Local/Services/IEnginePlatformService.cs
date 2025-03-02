using UnrealPluginManager.Local.Model.Engine;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// Provides an abstraction for platform-specific engine-related services.
/// </summary>
public interface IEnginePlatformService {
    /// <summary>
    /// Gets the file extension used for script files on the current platform.
    /// On Windows, this is typically "bat", while on POSIX-based systems (e.g., Linux, macOS), this is "sh".
    /// </summary>
    string ScriptFileExtension { get; }

    /// <summary>
    /// Retrieves a list of installed Unreal Engine versions available on the platform.
    /// </summary>
    /// <returns>A list of <see cref="InstalledEngine"/> representing the Unreal Engine installations.</returns>
    List<InstalledEngine> GetInstalledEngines();

}