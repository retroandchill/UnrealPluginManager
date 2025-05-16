namespace UnrealPluginManager.Core.Abstractions;

public class UnsupportedPlatformRegistry : IRegistry {
  public IRegistryKey LocalMachine => throw new PlatformNotSupportedException();
  public IRegistryKey CurrentUser => throw new PlatformNotSupportedException();
}