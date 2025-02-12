using System.IO.Abstractions;
using UnrealPluginManager.Cli.Model.Engine;

namespace UnrealPluginManager.Cli.Services;

public interface IEngineService {
    List<InstalledEngine> GetInstalledEngines();

    public Task<int> BuildPlugin(FileInfo pluginFile, string? engineVersion);
}