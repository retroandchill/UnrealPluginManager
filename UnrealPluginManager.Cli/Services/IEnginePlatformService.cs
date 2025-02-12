using UnrealPluginManager.Cli.Model.Engine;

namespace UnrealPluginManager.Cli.Services;

public interface IEnginePlatformService {

    string ScriptFileExtension { get; }

    List<InstalledEngine> GetInstalledEngines();

}