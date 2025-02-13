using System.Runtime.Versioning;

namespace UnrealPluginManager.Cli.Abstractions;

[SupportedOSPlatform("windows")]
public interface IRegistry {
    
    IRegistryKey LocalMachine { get; }
    
    IRegistryKey CurrentUser { get; }
    
}