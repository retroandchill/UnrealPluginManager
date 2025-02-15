using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.Versioning;
using Microsoft.Extensions.DependencyInjection;
using UnrealPluginManager.Cli.Abstractions;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Cli.Tests.Mocks;

namespace UnrealPluginManager.Cli.Tests.Services;

[SupportedOSPlatform("windows")]
public class WindowsEnginePlatformServiceTest {
    
    private ServiceProvider _serviceProvider;
    
    private const string ResourceFolder = "C:/dev/UnrealEngine/5.6_CustomBuild/Engine/Source/Runtime/Launch/Resources";
    
    [SetUp]
    public void Setup() {
        var services = new ServiceCollection();

        var mockFilesystem = new MockFileSystem(new Dictionary<string, MockFileData>());
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
    public void TestGetEngineVersionsWithFullSemVersion() {
        var platformService = _serviceProvider.GetRequiredService<IEnginePlatformService>();
        var fileSystem = _serviceProvider.GetRequiredService<IFileSystem>();
        fileSystem.Directory.CreateDirectory(ResourceFolder);
        using (var writeStream = fileSystem.File.CreateText(Path.Join(ResourceFolder, "Version.h"))) {
            writeStream.Write("#define ENGINE_MAJOR_VERSION 5\n#define ENGINE_MINOR_VERSION 6\n#define ENGINE_PATCH_VERSION 0");
        }

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
    
    [Test]
    public void TestGetEngineVersionsWithPartialSemVersion() {
        var platformService = _serviceProvider.GetRequiredService<IEnginePlatformService>();
        var fileSystem = _serviceProvider.GetRequiredService<IFileSystem>();
        fileSystem.Directory.CreateDirectory(ResourceFolder);
        using (var writeStream = fileSystem.File.CreateText(Path.Join(ResourceFolder, "Version.h"))) {
            writeStream.Write("#define ENGINE_MAJOR_VERSION 5\n#define ENGINE_MINOR_VERSION 6");
        }

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
            Assert.That(installedEngines[2].Version, Is.EqualTo(new Version(5, 6)));
            Assert.That(installedEngines[2].CustomBuild, Is.True);
        });
    }
    
    [Test]
    public void TestGetEngineVersionsWithMissingMajorVersion() {
        var platformService = _serviceProvider.GetRequiredService<IEnginePlatformService>();
        var fileSystem = _serviceProvider.GetRequiredService<IFileSystem>();
        fileSystem.Directory.CreateDirectory(ResourceFolder);
        using (var writeStream = fileSystem.File.CreateText(Path.Join(ResourceFolder, "Version.h"))) {
            writeStream.Write("#define ENGINE_MINOR_VERSION 6");
        }

        var installedEngines = platformService.GetInstalledEngines();
        Assert.That(installedEngines, Has.Count.EqualTo(2));
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
        });
    }
    
    [Test]
    public void TestGetEngineVersionsWithMissingMinorVersion() {
        var platformService = _serviceProvider.GetRequiredService<IEnginePlatformService>();
        var fileSystem = _serviceProvider.GetRequiredService<IFileSystem>();
        fileSystem.Directory.CreateDirectory(ResourceFolder);
        using (var writeStream = fileSystem.File.CreateText(Path.Join(ResourceFolder, "Version.h"))) {
            writeStream.Write("#define ENGINE_MAJOR_VERSION 5");
        }

        var installedEngines = platformService.GetInstalledEngines();
        Assert.That(installedEngines, Has.Count.EqualTo(2));
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
        });
    }
    
    [Test]
    public void TestGetEngineVersionsWithMissingVersionFile() {
        var platformService = _serviceProvider.GetRequiredService<IEnginePlatformService>();
        var fileSystem = _serviceProvider.GetRequiredService<IFileSystem>();

        var installedEngines = platformService.GetInstalledEngines();
        Assert.That(installedEngines, Has.Count.EqualTo(2));
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
        });
    }
    
}