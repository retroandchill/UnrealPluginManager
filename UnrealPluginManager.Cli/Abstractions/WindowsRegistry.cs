using System.Runtime.Versioning;

namespace UnrealPluginManager.Cli.Abstractions;

[SupportedOSPlatform("windows")]
public class WindowsRegistry : IRegistry {
    public IRegistryKey LocalMachine => new WindowsRegistryKey(Microsoft.Win32.Registry.LocalMachine);
    public IRegistryKey CurrentUser => new WindowsRegistryKey(Microsoft.Win32.Registry.CurrentUser);
}