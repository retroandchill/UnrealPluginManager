using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Semver;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Tests.Mocks;
using UnrealPluginManager.Local.Model.Engine;
using UnrealPluginManager.Local.Model.Plugins;
using UnrealPluginManager.Local.Services;

namespace UnrealPluginManager.Local.Tests.Services;

public partial class EngineServiceTest {
  private static readonly JsonSerializerOptions JsonOptions = new() {
      AllowTrailingCommas = true
  };

  private ServiceProvider _serviceProvider;
  private MockFileSystem _filesystem;
  private Mock<IEnginePlatformService> _enginePlatformService;
  private MockProcessRunner _processRunner;
  private Mock<IPluginService> _pluginService;
  private Mock<IPluginStructureService> _pluginStructureService;

  [SetUp]
  public void Setup() {
    var services = new ServiceCollection();

    _filesystem = new MockFileSystem(new Dictionary<string, MockFileData>());
    services.AddSingleton<IFileSystem>(_filesystem);

    _enginePlatformService = new Mock<IEnginePlatformService>();
    services.AddSingleton(_enginePlatformService.Object);
    _processRunner = new MockProcessRunner();
    services.AddSingleton<IProcessRunner>(_processRunner);
    _pluginService = new Mock<IPluginService>();
    services.AddSingleton(_pluginService.Object);
    _pluginStructureService = new Mock<IPluginStructureService>();
    services.AddSingleton(_pluginStructureService.Object);

    services.AddSingleton<IJsonService>(new JsonService(JsonOptions));
    services.AddSingleton<IEngineService, EngineService>();

    _serviceProvider = services.BuildServiceProvider();
  }

  [TearDown]
  public void TearDown() {
    _serviceProvider.Dispose();
  }

  [Test]
  public async Task TestBuildPlugin() {
    // Setup mock engine environments
    var installedEngines = new List<InstalledEngine> {
        new("5.4", new Version(5, 4), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.4")),
        new("5.5", new Version(5, 5), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.5")),
        new("5.6_Custom", new Version(5, 6), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.6_Custom"), true),
    };
    _enginePlatformService.Setup(x => x.ScriptFileExtension).Returns("bat");
    _enginePlatformService.Setup(x => x.GetInstalledEngines()).Returns(installedEngines);

    // Setup source plugin
    const string pluginPath = "C:/dev/Plugins/MyPlugin";
    _filesystem.Directory.CreateDirectory(pluginPath);
    var pluginFile = Path.GetFullPath(Path.Join(pluginPath, "MyPlugin.uplugin"));

    // Setup process runner mock for UAT
    var batchFilePath = Path.GetFullPath("C:/dev/UnrealEngine/5.5/Engine/Build/BatchFiles/RunUAT.bat");
    _processRunner.Setup(x => x.RunProcess(
            It.Is<string>(y => y == batchFilePath),
            It.Is<string[]>(y => y.Length == 3 &&
                                 y[0] == "BuildPlugin" &&
                                 y[1] == $"-Plugin=\"{pluginFile}\"" &&
                                 y[2].StartsWith("-package=\"")), It.IsAny<string>()))
        .ReturnsAsync(0);

    var engineService = _serviceProvider.GetRequiredService<IEngineService>();
    var destination =
        _filesystem.DirectoryInfo.New(
            "C:/dev/UnrealEngine/5.5/Engine/Plugins/Marketplace/.UnrealPluginManager/TestPlugin");

    // Test plugin build with specific engine version and platforms
    var result = await engineService.BuildPlugin(
        _filesystem.FileInfo.New(pluginFile),
        destination,
        "5.5",
        new List<string> {
            "Win64"
        });

    Assert.That(result, Is.EqualTo(0));

    // Verify process runner was called correctly
    _processRunner.Verify(x => x.RunProcess(
            It.Is<string>(y => y == batchFilePath),
            It.Is<string[]>(y =>
                y[0] == "BuildPlugin" &&
                y[1] == $"-Plugin=\"{pluginFile}\"" &&
                y[2] == $"-package=\"{destination.FullName}\""),
            It.Is<string?>(y => y == null)),
        Times.Once());

  }

  [Test]
  public async Task TestInstallPlugin() {
    var installedEngines = new List<InstalledEngine> {
        new("5.4", new Version(5, 4), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.4")),
        new("5.5", new Version(5, 5), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.5")),
        new("5.6_Custom", new Version(5, 6), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.6_Custom"), true),
    };
    _enginePlatformService.Setup(x => x.GetInstalledEngines()).Returns(installedEngines);

    const string pluginPath = "C:/dev/Plugins/MyPlugin";
    var dirName = Path.GetFullPath(pluginPath);
    Assert.That(dirName, Is.Not.Null);
    _filesystem.Directory.CreateDirectory(dirName);

    var descriptor = new PluginDescriptor {
        Version = 1,
        FriendlyName = "My Plugin",
        VersionName = new SemVersion(1, 0, 0),
        Installed = false
    };
    var descriptorJson = JsonSerializer.Serialize(descriptor);
    await _filesystem.File.WriteAllTextAsync(Path.Join(dirName, "MyPlugin.uplugin"), descriptorJson);

    var engineService = _serviceProvider.GetRequiredService<IEngineService>();
    var installedPluginVersion = await engineService.GetInstalledPluginVersion("MyPlugin", "5.4");
    Assert.That(installedPluginVersion.IsNone, Is.True);
    var source = _filesystem.DirectoryInfo.New(pluginPath);
    engineService.InstallPlugin("MyPlugin", source, "5.4");
    Assert.That(_filesystem.Directory.Exists(
            Path.Join("C:/dev/UnrealEngine/5.4/Engine/Plugins/Marketplace/.UnrealPluginManager/MyPlugin")),
        Is.True);

    installedPluginVersion = await engineService.GetInstalledPluginVersion("MyPlugin", "5.4");
    Assert.That(installedPluginVersion.IsSome, Is.True);
  }

  [GeneratedRegex("-Package=\"(.*)\"", RegexOptions.IgnoreCase)]
  private static partial Regex PackageRegex();

  [Test]
  public async Task TestGetInstalledPlugins() {
    var installedEngines = new List<InstalledEngine> {
        new("5.5", new Version(5, 5), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.5")),
    };
    _enginePlatformService.Setup(x => x.GetInstalledEngines()).Returns(installedEngines);

    var newDir =
        _filesystem.Directory.CreateDirectory(
            "C:/dev/UnrealEngine/5.5/Engine/Plugins/Marketplace/.UnrealPluginManager");
    var descriptor1 = new PluginDescriptor {
        Version = 1,
        FriendlyName = "My Plugin",
        VersionName = new SemVersion(1, 0, 0),
        Installed = true
    };
    var subDir1 = newDir.CreateSubdirectory("MyPlugin");
    await _filesystem.File.WriteAllTextAsync(Path.Join(subDir1.FullName, "MyPlugin.uplugin"),
        JsonSerializer.Serialize(descriptor1));

    var descriptor2 = new PluginDescriptor {
        Version = 1,
        FriendlyName = "Second Plugin",
        VersionName = new SemVersion(2, 4, 3),
        Installed = true
    };
    var subDir2 = newDir.CreateSubdirectory("SecondPlugin");
    await _filesystem.File.WriteAllTextAsync(Path.Join(subDir2.FullName, "SecondPlugin.uplugin"),
        JsonSerializer.Serialize(descriptor2));

    var descriptor3 = new PluginDescriptor {
        Version = 1,
        FriendlyName = "Another Plugin",
        VersionName = new SemVersion(1, 1, 3),
        Installed = true
    };
    var subDir3 = newDir.CreateSubdirectory("AnotherPlugin");
    await _filesystem.File.WriteAllTextAsync(Path.Join(subDir3.FullName, "AnotherPlugin.uplugin"),
        JsonSerializer.Serialize(descriptor3));

    _pluginStructureService.Setup(x => x.GetInstalledBinaries(It.IsAny<IDirectoryInfo>()))
        .Returns(["Win64"]);

    var engineService = _serviceProvider.GetRequiredService<IEngineService>();
    var pluginVersions = await engineService.GetInstalledPlugins("5.5")
        .ToListAsync();

    Assert.That(pluginVersions, Has.Count.EqualTo(3));
    Assert.That(pluginVersions, Does.Contain(new InstalledPlugin("MyPlugin", new SemVersion(1, 0, 0), ["Win64"])));
    Assert.That(pluginVersions, Does.Contain(new InstalledPlugin("SecondPlugin", new SemVersion(2, 4, 3), ["Win64"])));
    Assert.That(pluginVersions, Does.Contain(new InstalledPlugin("AnotherPlugin", new SemVersion(1, 1, 3), ["Win64"])));
  }
}