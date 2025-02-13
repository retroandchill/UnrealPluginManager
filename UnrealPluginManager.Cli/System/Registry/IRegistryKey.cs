using System.Runtime.Versioning;

namespace UnrealPluginManager.Cli.System.Registry;

[SupportedOSPlatform("windows")]
public interface IRegistryKey {
    
    IRegistryKey? OpenSubKey(string name);
    
    string[] GetSubKeyNames();
    
    string[] GetValueNames();
    
    T? GetValue<T>(string name);
    
}