using System.Runtime.Versioning;

namespace UnrealPluginManager.Cli.System.Registry;

[SupportedOSPlatform("windows")]
public interface IRegistry {
    
    IRegistryKey LocalMachine { get; }
    
    IRegistryKey CurrentUser { get; }
    
}