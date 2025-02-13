using System.IO.Abstractions;
using LanguageExt;
using Semver;
using UnrealPluginManager.Cli.Model.Engine;

namespace UnrealPluginManager.Cli.Services;

public interface IEngineService {
    List<InstalledEngine> GetInstalledEngines();

    public Task<int> BuildPlugin(FileInfo pluginFile, string? engineVersion);
    
    public Task<int> InstallPlugin(string pluginName, SemVersionRange pluginVersion, string? engineVersion);

}