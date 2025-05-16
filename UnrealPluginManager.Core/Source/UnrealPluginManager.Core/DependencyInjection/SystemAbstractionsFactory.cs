using System.IO.Abstractions;
using UnrealPluginManager.Core.Abstractions;

namespace UnrealPluginManager.Core.DependencyInjection;

/// <summary>
/// A factory for providing platform-independent implementations of system-level abstractions,
/// such as file system operations, environment variables, process management, and registry access.
/// </summary>
/// <remarks>
/// This class enables decoupling of high-level application logic from specific implementations of
/// these system-level functionalities, promoting testability and platform independence.
/// </remarks>
public class SystemAbstractionsFactory : ISystemAbstractionsFactory {
  /// <inheritdoc />
  public IFileSystem CreateFileSystem() {
    return new FileSystem();
  }

  /// <inheritdoc />
  public IEnvironment CreateEnvironment() {
    return new SystemEnvironment();
  }

  /// <inheritdoc />
  public IProcessRunner CreateProcessRunner() {
    return new ProcessRunner();
  }

  /// <inheritdoc />
  public IRegistry CreateRegistry() {
    return OperatingSystem.IsWindows() ? new WindowsRegistry() : new UnsupportedPlatformRegistry();
  }
}