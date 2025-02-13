using System.Runtime.Versioning;
using Microsoft.Win32;

namespace UnrealPluginManager.Cli.Abstractions;

[SupportedOSPlatform("windows")]
public interface IRegistry {
    IRegistryKey CurrentUser { get; }
    
    IRegistryKey LocalMachine { get; }
    
    IRegistryKey ClassesRoot { get; }
    
    IRegistryKey Users { get; }

    IRegistryKey PerformanceData { get; }
    
    IRegistryKey CurrentConfig { get; }

    IRegistryKey GetBaseKeyFromKeyName(string keyName, out string subKeyName);

    object? GetValue(string keyName, string? valueName, object? defaultValue);

    void SetValue(string keyName, string? valueName, object value);

    void SetValue(string keyName, string? valueName, object value, RegistryValueKind valueKind);
}