using System.IO.Abstractions;
using Jab;
using UnrealPluginManager.Core.Abstractions;

namespace UnrealPluginManager.Core.DependencyInjection;

[ServiceProviderModule]
[Singleton(typeof(IFileSystem), typeof(FileSystem))]
[Singleton(typeof(IEnvironment), typeof(SystemEnvironment))]
[Singleton(typeof(IProcessRunner), typeof(ProcessRunner))]
[Singleton(typeof(IRegistry), Factory = nameof(CreateRegistry))]
public interface ISystemAbstractionsModule {
  public static IRegistry CreateRegistry() {
    return OperatingSystem.IsWindows() ? new WindowsRegistry() : new UnsupportedPlatformRegistry();
  }
}