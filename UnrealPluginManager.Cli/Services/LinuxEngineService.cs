using System.Runtime.Versioning;

namespace UnrealPluginManager.Cli.Services;

[SupportedOSPlatform("linux")]
public class LinuxEngineService : IEngineService {
    public List<string> GetInstalledEngines() {
        throw new NotImplementedException();
    }
}