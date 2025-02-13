using System.Runtime.Versioning;
using UnrealPluginManager.Cli.Services;

namespace UnrealPluginManager.Cli.System.Registry;

[SupportedOSPlatform("windows")]
public class WindowsRegistry : IRegistry {
    public IRegistryKey LocalMachine => new WindowsRegistryKey(Microsoft.Win32.Registry.LocalMachine);
    public IRegistryKey CurrentUser => new WindowsRegistryKey(Microsoft.Win32.Registry.CurrentUser);
}