using System.IO.Abstractions;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Win32;
using UnrealPluginManager.Cli.Model.Engine;
using UnrealPluginManager.Cli.Utils;

namespace UnrealPluginManager.Cli.Services;

[SupportedOSPlatform("windows")]
public class WindowsEnginePlatformService(IFileSystem fileSystem) : IEnginePlatformService {
    public string ScriptFileExtension => "bat";

    public List<InstalledEngine> GetInstalledEngines() {
        return GetInstalledEnginesFromRegistry(@"Software\EpicGames\Unreal Engine")
            .Concat(GetCustomBuiltEnginesFromRegistry(@"Software\Epic Games\Unreal Engine\Builds"))
            .ToList();
    }

    private IEnumerable<InstalledEngine> GetInstalledEnginesFromRegistry(string registryKey) {
        var engineInstallations = Registry.LocalMachine.OpenSubKey(registryKey);
        if (engineInstallations is null) {
            return [];
        }
        
        return engineInstallations.GetSubKeyNames()
            .Select(s => (Key: s, Value: engineInstallations.OpenSubKey(s)))
            .Where(s => s.Value is not null)
            .Select(s => (Name: s.Key, Directory: s.Value!.GetValue("InstalledDirectory") as string))
            .Where(s => s.Directory is not null)
            .Select((x, i) => new InstalledEngine(x.Name,
                Version.Parse(x.Name), fileSystem.DirectoryInfo.New(x.Directory!)));
    }
    
    private IEnumerable<InstalledEngine> GetCustomBuiltEnginesFromRegistry(string registryKey) {
        var engineInstallations = Registry.CurrentUser.OpenSubKey(registryKey);
        if (engineInstallations is null) {
            return [];
        }
        
        return engineInstallations.GetValueNames()
            .Select(s => (Name: s, Directory: engineInstallations.GetValue(s) as string))
            .Where(s => s.Directory is not null)
            .Select((x, i) => new InstalledEngine(x.Name,
                fileSystem.GetEngineVersion(x.Directory!), fileSystem.DirectoryInfo.New(x.Directory!), true));
    }
}