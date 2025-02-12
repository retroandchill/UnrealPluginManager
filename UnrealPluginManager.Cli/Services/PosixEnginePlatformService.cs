using System.Runtime.Versioning;
using UnrealPluginManager.Cli.Model.Engine;

namespace UnrealPluginManager.Cli.Services;

public class PosixEnginePlatformService : IEnginePlatformService {
    public string ScriptFileExtension => "sh";

    public List<InstalledEngine> GetInstalledEngines() {
        throw new NotImplementedException();
    }
}