using System.Runtime.Versioning;
using System.Security.AccessControl;

namespace UnrealPluginManager.Cli.Abstractions;

[SupportedOSPlatform("windows")]
public interface IRegistrySecurity {
    
    RegistrySecurity RegistrySecurity => throw new NotSupportedException();

}