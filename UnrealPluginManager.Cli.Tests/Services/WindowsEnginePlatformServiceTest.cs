using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UnrealPluginManager.Cli.Abstractions;
using UnrealPluginManager.Cli.Model.Engine;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Cli.Tests.Mocks;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Cli.Tests.Services;

[SupportedOSPlatform("windows")]
public class WindowsEnginePlatformServiceTest {
    
    private ServiceProvider _serviceProvider;
    
    [SetUp]
    public void Setup() {
        var services = new ServiceCollection();

        var mockFilesystem = new MockFileSystem(new Dictionary<string, MockFileData>());
        const string resourceFolder = "C:/dev/UnrealEngine/5.6_CustomBuild/Engine/Source/Runtime/Launch/Resources";
        mockFilesystem.AddDirectory(resourceFolder);
        using (var writeStream = mockFilesystem.File.CreateText(Path.Join(resourceFolder, "Version.h"))) {
            writeStream.Write("#define ENGINE_MAJOR_VERSION 5\n#define ENGINE_MINOR_VERSION 6\n#define ENGINE_PATCH_VERSION 0");
        }
        services.AddSingleton<IFileSystem>(mockFilesystem);

        var mockRegistry = new MockRegistry();
        var ue54 = mockRegistry.LocalMachine.OpenOrAddSubKey(@"SOFTWARE\EpicGames\Unreal Engine\5.4");
        ue54.SetValue("InstalledDirectory", @"C:\Program Files\Epic Games\UE_5.4");
        var ue55 = mockRegistry.LocalMachine.OpenOrAddSubKey(@"SOFTWARE\EpicGames\Unreal Engine\5.5");
        ue55.SetValue("InstalledDirectory", @"C:\Program Files\Epic Games\UE_5.5");
        
        var customBuild = mockRegistry.CurrentUser.OpenOrAddSubKey(@"SOFTWARE\Epic Games\Unreal Engine\Builds");
        customBuild.SetValue("5.6_Custom", @"C:\dev\UnrealEngine\5.6_CustomBuild");
        services.AddSingleton<IRegistry>(mockRegistry);

        services.AddSingleton<IEnginePlatformService, WindowsEnginePlatformService>();
        
        _serviceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public void TearDown() {
        _serviceProvider.Dispose();
    }

    [Test]
    public void TestGetEngineVersions() {
        var platformService = _serviceProvider.GetRequiredService<IEnginePlatformService>();

        var installedEngines = platformService.GetInstalledEngines();
        Assert.That(installedEngines, Has.Count.EqualTo(3));
        Assert.Multiple(() =>
        {
            Assert.That(installedEngines[0].Name, Is.EqualTo("5.4"));
            Assert.That(installedEngines[0].DisplayName, Is.EqualTo("5.4: Installed"));
            Assert.That(installedEngines[0].Version, Is.EqualTo(new Version(5, 4)));
            Assert.That(installedEngines[0].CustomBuild, Is.False);
            
            Assert.That(installedEngines[1].Name, Is.EqualTo("5.5"));
            Assert.That(installedEngines[1].DisplayName, Is.EqualTo("5.5: Installed"));
            Assert.That(installedEngines[1].Version, Is.EqualTo(new Version(5, 5)));
            Assert.That(installedEngines[1].CustomBuild, Is.False);
            
            Assert.That(installedEngines[2].Name, Is.EqualTo("5.6_Custom"));
            Assert.That(installedEngines[2].DisplayName, Is.EqualTo("5.6_Custom: Custom Build"));
            Assert.That(installedEngines[2].Version, Is.EqualTo(new Version(5, 6, 0)));
            Assert.That(installedEngines[2].CustomBuild, Is.True);
        });
    }
    
    
}