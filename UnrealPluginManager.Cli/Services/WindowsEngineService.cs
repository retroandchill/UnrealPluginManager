using System.Runtime.Versioning;
using Microsoft.Win32;

namespace UnrealPluginManager.Cli.Services;

[SupportedOSPlatform("windows")]
public class WindowsEngineService : IEngineService {
    
    public List<string> GetInstalledEngines() {
        var engineInstallations = Registry.LocalMachine.OpenSubKey(@"Software\EpicGames\Unreal Engine");
        if (engineInstallations is null) {
            return [];
        }
        
        return engineInstallations.GetSubKeyNames()
            .Select(s => engineInstallations.OpenSubKey(s))
            .Where(s => s is not null)
            .Select(x => x.GetValue("InstalledDirectory") as string)
            .Where(x => x is not null)
            .ToList();
    }
}