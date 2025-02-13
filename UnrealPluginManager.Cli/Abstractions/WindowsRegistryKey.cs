using System.Runtime.Versioning;
using Microsoft.Win32;

namespace UnrealPluginManager.Cli.Abstractions;


[SupportedOSPlatform("windows")]
public record WindowsRegistryKey(RegistryKey RegistryKey) : IRegistryKey {
    public IRegistryKey? OpenSubKey(string name) {
        var key = RegistryKey.OpenSubKey(name);
        return key is not null ? new WindowsRegistryKey(key) : null;
    }

    public string[] GetSubKeyNames() {
        return RegistryKey.GetSubKeyNames();
    }

    public string[] GetValueNames() {
        return RegistryKey.GetValueNames();
    }

    public T? GetValue<T>(string name) {
        return (T?) RegistryKey.GetValue(name);
    }
}