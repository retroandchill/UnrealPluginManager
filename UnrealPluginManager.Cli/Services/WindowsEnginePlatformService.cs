using System.IO.Abstractions;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using Microsoft.Win32;
using UnrealPluginManager.Cli.Model.Engine;

namespace UnrealPluginManager.Cli.Services;

[SupportedOSPlatform("windows")]
public class WindowsEnginePlatformService(IFileSystem fileSystem) : IEnginePlatformService {
    public string ScriptFileExtension => "bat";

    public List<InstalledEngine> GetInstalledEngines() {
        return GetInstalledEnginesFromRegistry(@"Software\EpicGames\Unreal Engine", false)
            .Concat(GetInstalledEnginesFromRegistry(@"Software\Epic Games\Unreal Engine\Builds", true))
            .ToList();
    }

    private IEnumerable<InstalledEngine> GetInstalledEnginesFromRegistry(string registryKey, bool custom) {
        var engineInstallations = Registry.LocalMachine.OpenSubKey(registryKey);
        if (engineInstallations is null) {
            return [];
        }
        
        return engineInstallations.GetSubKeyNames()
            .Select(s => new EngineKey(s, custom))
            .Select(s => (s.Name, Key: engineInstallations.OpenSubKey(s.Name), s.Custom))
            .Where(s => s.Key is not null)
            .Select(s => (s.Name, s.Key, Directory: s.Key!.GetValue("InstalledDirectory") as string, s.Custom))
            .Where(s => s.Directory is not null)
            .Select((x, i) => new InstalledEngine {
                Key = x.Custom ? $"{x.Name}-c{i + 1})" : x.Name,
                Version = Version.Parse(x.Name),
                Name = x.Custom ? $"{x.Name} (Custom Build {i + 1})" : x.Name,
                Directory = fileSystem.DirectoryInfo.New(x.Directory!),
                CustomBuild = x.Custom
            });
    }
}