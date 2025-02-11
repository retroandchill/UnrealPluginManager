using System.Runtime.Versioning;
using UnrealPluginManager.Cli.Model.Engine;

namespace UnrealPluginManager.Cli.Services;

public class PosixEngineService : IEngineService {
    public List<InstalledEngine> GetInstalledEngines() {
        throw new NotImplementedException();
    }
}