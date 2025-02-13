using System.Runtime.Versioning;

namespace UnrealPluginManager.Cli.Abstractions;

[SupportedOSPlatform("windows")]
public interface IRegistryKey {
    
    IRegistryKey? OpenSubKey(string name);
    
    string[] GetSubKeyNames();
    
    string[] GetValueNames();
    
    T? GetValue<T>(string name);
    
}