using System.IO.Abstractions;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Win32;
using UnrealPluginManager.Cli.Model.Engine;
using UnrealPluginManager.Cli.Utils;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Cli.Services;

/// <summary>
/// Provides platform-specific implementation for engine-related services on Windows.
/// </summary>
/// <remarks>
/// This service interacts with the Windows registry to retrieve information about
/// installed Unreal Engine versions, including both official installations and custom builds.
/// </remarks>
/// <example>
/// The service uses the Windows registry paths to fetch engine data, such as:
/// - Software\EpicGames\Unreal Engine
/// - Software\Epic Games\Unreal Engine\Builds
/// It also defines the appropriate script file extension to be used on Windows platforms.
/// </example>
/// <seealso cref="IEnginePlatformService"/>
[SupportedOSPlatform("windows")]
public class WindowsEnginePlatformService(IFileSystem fileSystem, IRegistry registry) : IEnginePlatformService {
    /// <inheritdoc />
    public string ScriptFileExtension => "bat";

    /// <inheritdoc />
    public List<InstalledEngine> GetInstalledEngines() {
        return GetInstalledEnginesFromRegistry(@"Software\EpicGames\Unreal Engine")
            .Concat(GetCustomBuiltEnginesFromRegistry(@"Software\Epic Games\Unreal Engine\Builds"))
            .ToList();
    }

    private IEnumerable<InstalledEngine> GetInstalledEnginesFromRegistry(string registryKey) {
        var engineInstallations = registry.LocalMachine.OpenSubKey(registryKey);
        if (engineInstallations is null) {
            return [];
        }
        
        return engineInstallations.GetSubKeyNames()
            .Select(s => (Key: s, Value: engineInstallations.OpenSubKey(s)))
            .Where(s => s.Value is not null)
            .Select(s => (Name: s.Key, Directory: s.Value!.GetValue<string>("InstalledDirectory")))
            .Where(s => s.Directory is not null)
            .Select((x, i) => new InstalledEngine(x.Name,
                Version.Parse(x.Name), fileSystem.DirectoryInfo.New(x.Directory!)));
    }
    
    private IEnumerable<InstalledEngine> GetCustomBuiltEnginesFromRegistry(string registryKey) {
        var engineInstallations = registry.CurrentUser.OpenSubKey(registryKey);
        if (engineInstallations is null) {
            return [];
        }
        
        return engineInstallations.GetValueNames()
            .Select(s => (Name: s, Directory: engineInstallations.GetValue<string>(s)))
            .Where(s => s.Directory is not null)
            .SelectValid((x, i) => new InstalledEngine(x.Name,
                fileSystem.GetEngineVersion(x.Directory!), fileSystem.DirectoryInfo.New(x.Directory!), true));
    }
}