using System.Runtime.Versioning;

namespace UnrealPluginManager.Cli.Services;

[SupportedOSPlatform("macos")]
public class MacEngineService : IEngineService {
    public List<string> GetInstalledEngines() {
        throw new NotImplementedException();
    }
}