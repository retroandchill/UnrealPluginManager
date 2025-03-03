using UnrealPluginManager.Local.Model.Engine;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// A platform-specific implementation of <see cref="IEnginePlatformService"/> for POSIX-based systems (e.g., Linux, macOS).
/// </summary>
/// <remarks>
/// This service provides functionality to manage Unreal Engine installations and related tasks
/// on platforms that adhere to the POSIX standard.
/// </remarks>
public class PosixEnginePlatformService : IEnginePlatformService {
  /// <inheritdoc />
  public string ScriptFileExtension => "sh";

  /// <inheritdoc />
  public List<InstalledEngine> GetInstalledEngines() {
    throw new NotImplementedException();
  }
}