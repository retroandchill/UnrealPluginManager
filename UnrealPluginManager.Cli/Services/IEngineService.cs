using UnrealPluginManager.Cli.Model.Engine;

namespace UnrealPluginManager.Cli.Services;

public interface IEngineService {
    
    List<InstalledEngine> GetInstalledEngines();
    
}