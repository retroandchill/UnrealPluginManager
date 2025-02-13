using System.Runtime.Versioning;
using System.Security.AccessControl;

namespace UnrealPluginManager.Cli.Abstractions;

[SupportedOSPlatform("windows")]
internal record RegistrySecurityWrapper(RegistrySecurity RegistrySecurity) : IRegistrySecurity {
    
}