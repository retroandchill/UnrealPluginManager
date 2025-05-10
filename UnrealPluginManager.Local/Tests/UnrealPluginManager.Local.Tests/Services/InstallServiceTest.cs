using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using LanguageExt;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Semver;
using UnrealPluginManager.Core.Mappers;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Project;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Local.Model.Cache;
using UnrealPluginManager.Local.Model.Engine;
using UnrealPluginManager.Local.Model.Installation;
using UnrealPluginManager.Local.Model.Plugins;
using UnrealPluginManager.Local.Services;

namespace UnrealPluginManager.Local.Tests.Services;

public class InstallServiceTest {
  private ServiceProvider _serviceProvider;
  private MockFileSystem _filesystem;
  private Mock<IEngineService> _engineService;
  private Mock<IPluginManagementService> _pluginManagementService;
  private IInstallService _installService;

  [SetUp]
  public void Setup() {
    var services = new ServiceCollection();

    _filesystem = new MockFileSystem(new Dictionary<string, MockFileData>());
    services.AddSingleton<IFileSystem>(_filesystem);

    _engineService = new Mock<IEngineService>();
    services.AddSingleton(_engineService.Object);

    _pluginManagementService = new Mock<IPluginManagementService>();
    services.AddSingleton(_pluginManagementService.Object);

    services.AddSingleton<IJsonService>(new JsonService(JsonSerializerOptions.Default));
    services.AddSingleton<IInstallService, InstallService>();
    _serviceProvider = services.BuildServiceProvider();
    _installService = _serviceProvider.GetRequiredService<IInstallService>();
  }

  [TearDown]
  public void TearDown() {
    _serviceProvider.Dispose();
  }

  [Test]
  public async Task TestInstallPlugin() {
    // Setup test data
    const string pluginName = "TestPlugin";
    var targetVersion = SemVersionRange.AtLeast(new SemVersion(2, 0, 0));
    const string engineVersion = "5.5";
    var platforms = new List<string> {
        "Win64"
    };

    // Case 1: Plugin already installed with matching version
    _engineService.Setup(x => x.GetInstalledPluginVersion(pluginName, engineVersion))
        .ReturnsAsync(new SemVersion(2, 1, 0));

    var result = await _installService.InstallPlugin(pluginName, targetVersion, engineVersion, platforms);

    Assert.That(result, Is.Empty);
    _pluginManagementService.Verify(x => x.FindTargetPlugin(It.IsAny<string>(),
        It.IsAny<SemVersionRange>(), It.IsAny<string>()), Times.Never);

    // Case 2: Plugin needs installation
    _engineService.Setup(x => x.GetInstalledPluginVersion(pluginName, engineVersion))
        .ReturnsAsync(new SemVersion(1, 0, 0)); // Version outside target range

    var targetPlugin = new PluginVersionInfo {
        PluginId = Guid.NewGuid(),
        Name = pluginName,
        VersionId = Guid.NewGuid(),
        Version = new SemVersion(2, 0, 0),
        Dependencies = []
    };

    var pluginSummary = new PluginSummary {
        PluginId = targetPlugin.PluginId,
        Name = pluginName,
        VersionId = targetPlugin.VersionId,
        Version = targetPlugin.Version
    };

    _pluginManagementService.Setup(x => x.FindTargetPlugin(pluginName, targetVersion, engineVersion))
        .ReturnsAsync(targetPlugin);

    _pluginManagementService.Setup(x => x.GetPluginsToInstall(targetPlugin, engineVersion))
        .ReturnsAsync(new List<PluginSummary> {
            pluginSummary
        });

    _engineService.Setup(x => x.GetInstalledPlugins(engineVersion))
        .Returns(new List<InstalledPlugin> {
            new(pluginName, new SemVersion(1, 0, 0), new List<string> {
                "Win64"
            })
        }.ToAsyncEnumerable());

    _engineService.Setup(x => x.GetInstalledEngine(engineVersion))
        .Returns(new InstalledEngine(engineVersion, new Version(5, 5), _filesystem.DirectoryInfo.New("some/path")));

    var pluginBuildInfo = new PluginBuildInfo {
        PluginName = pluginName,
        PluginVersion = new SemVersion(2, 0, 0),
        DirectoryName = "cache/path/TestPlugin",
        EngineVersion = engineVersion,
        BuiltOn = DateTimeOffset.Now
    };

    _pluginManagementService.Setup(x => x.FindLocalPlugin(
            pluginName, new SemVersion(2, 0, 0), engineVersion, platforms))
        .ReturnsAsync(pluginBuildInfo);

    var changes = await _installService.InstallPlugin(pluginName, targetVersion, engineVersion, platforms);

    Assert.Multiple(() => {
      Assert.That(changes, Has.Count.EqualTo(1));
      Assert.That(changes[0].PluginName, Is.EqualTo(pluginName));
      Assert.That(changes[0].OldVersion, Is.EqualTo(new SemVersion(1, 0, 0)));
      Assert.That(changes[0].NewVersion, Is.EqualTo(new SemVersion(2, 0, 0)));
    });

    _engineService.Verify(x => x.InstallPlugin(
            pluginName,
            It.Is<IDirectoryInfo>(d => d.FullName == pluginBuildInfo.DirectoryName),
            engineVersion),
        Times.Once);
  }

  [Test]
  public async Task TestInstallPlugin_SkipsWhenPlatformsMatch() {
    const string pluginName = "TestPlugin";
    var targetVersion = SemVersionRange.Parse("2.0.0");
    const string engineVersion = "5.5";
    var platforms = new List<string> {
        "Win64"
    };

    var pluginSummary = new PluginSummary {
        PluginId = Guid.NewGuid(),
        Name = pluginName,
        VersionId = Guid.NewGuid(),
        Version = new SemVersion(2, 0, 0)
    };

    _engineService.Setup(x => x.GetInstalledPlugins(engineVersion))
        .Returns(new List<InstalledPlugin> {
            new(pluginName, new SemVersion(2, 0, 0), new List<string> {
                "Win64"
            })
        }.ToAsyncEnumerable());

    _pluginManagementService.Setup(x => x.GetPluginsToInstall(It.IsAny<PluginVersionInfo>(), engineVersion))
        .ReturnsAsync(new List<PluginSummary> {
            pluginSummary
        });

    var changes = await _installService.InstallPlugin(pluginName, targetVersion, engineVersion, platforms);

    Assert.That(changes, Is.Empty);
    _engineService.Verify(x => x.InstallPlugin(
            It.IsAny<string>(),
            It.IsAny<IDirectoryInfo>(),
            It.IsAny<string>()),
        Times.Never);
  }

  [Test]
  public async Task TestInstallPlugin_DownloadsWhenNotCached() {
    const string pluginName = "TestPlugin";
    var targetVersion = SemVersionRange.Parse("2.0.0");
    const string engineVersion = "5.5";
    var platforms = new List<string> {
        "Win64"
    };

    var targetPlugin = new PluginVersionInfo {
        PluginId = Guid.NewGuid(),
        Name = pluginName,
        VersionId = Guid.NewGuid(),
        Version = new SemVersion(2, 0, 0),
        Dependencies = [],
        RemoteIndex = 1
    };

    _pluginManagementService.Setup(x => x.FindTargetPlugin(pluginName, targetVersion, engineVersion))
        .ReturnsAsync(targetPlugin);

    _pluginManagementService.Setup(x => x.GetPluginsToInstall(targetPlugin, engineVersion))
        .ReturnsAsync(new List<PluginSummary> {
            targetPlugin.ToPluginSummary()
        });

    _engineService.Setup(x => x.GetInstalledPlugins(engineVersion))
        .Returns(AsyncEnumerable.Empty<InstalledPlugin>());

    _engineService.Setup(x => x.GetInstalledEngine(engineVersion))
        .Returns(new InstalledEngine(engineVersion, new Version(5, 5), _filesystem.DirectoryInfo.New("some/path")));

    var downloadedPlugin = new PluginBuildInfo {
        PluginName = pluginName,
        PluginVersion = new SemVersion(2, 0, 0),
        DirectoryName = "downloaded/path/TestPlugin",
        EngineVersion = engineVersion,
        BuiltOn = DateTimeOffset.Now
    };

    _pluginManagementService.Setup(x => x.FindLocalPlugin(
            pluginName, new SemVersion(2, 0, 0), engineVersion, platforms))
        .ReturnsAsync(Option<PluginBuildInfo>.None);

    _pluginManagementService.Setup(x => x.DownloadPlugin(
            pluginName, new SemVersion(2, 0, 0), 1, engineVersion, platforms))
        .ReturnsAsync(downloadedPlugin);

    var changes = await _installService.InstallPlugin(pluginName, targetVersion, engineVersion, platforms);

    Assert.That(changes, Has.Count.EqualTo(1));
    _engineService.Verify(x => x.InstallPlugin(
            pluginName,
            It.Is<IDirectoryInfo>(d => d.FullName == downloadedPlugin.DirectoryName),
            engineVersion),
        Times.Once);
  }

  [Test]
  public async Task TestInstallRequirements_ProjectDescriptor() {
    var projectDescriptor = new ProjectDescriptor {
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "Plugin1",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.AllRelease
            },
            new PluginReferenceDescriptor {
                Name = "Plugin2",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.AllRelease
            },
            new PluginReferenceDescriptor {
                Name = "Plugin3",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.AllRelease
            }
        ]
    };
    _filesystem.AddFile("Project.uproject", new MockFileData(JsonSerializer.Serialize(projectDescriptor)));

    _pluginManagementService.Setup(x => x.GetPluginsToInstall(It.IsAny<IDependencyChainNode>(), It.IsAny<string>()))
        .ReturnsAsync(new List<PluginSummary> {
            new() {
                PluginId = Guid.NewGuid(),
                Name = "Plugin1",
                VersionId = Guid.NewGuid(),
                Version = new SemVersion(1, 0, 0),
            },
            new() {
                PluginId = Guid.NewGuid(),
                Name = "Plugin2",
                VersionId = Guid.NewGuid(),
                Version = new SemVersion(1, 1, 0),
            },
            new() {
                PluginId = Guid.NewGuid(),
                Name = "Plugin3",
                VersionId = Guid.NewGuid(),
                Version = new SemVersion(1, 3, 0),
            }
        });

    var installedPlugins = new List<InstalledPlugin> {
        new("Plugin1", new SemVersion(1, 0, 0), ["Win64"]),
        new("Plugin2", new SemVersion(1, 0, 0), ["Win64"]),
        new("Plugin3", new SemVersion(1, 3, 0), []),
    };
    _engineService.Setup(x => x.GetInstalledEngine("5.5"))
        .Returns(new InstalledEngine("5.5", new Version(5, 5), null!));
    _engineService.Setup(x => x.GetInstalledPlugins("5.5"))
        .Returns(installedPlugins.ToAsyncEnumerable());

    var plugin2 = new PluginBuildInfo {
        PluginName = "Plugin2",
        PluginVersion = new SemVersion(1, 0, 0),
        EngineVersion = "5.5",
        DirectoryName = "C:/UnrealEngine/Engine/Plugins/Runtime/Plugin2/Content/Plugin2",
        BuiltOn = DateTime.Now
    };
    var plugin3 = new PluginBuildInfo {
        PluginName = "Plugin2",
        PluginVersion = new SemVersion(1, 1, 3),
        EngineVersion = "5.5",
        DirectoryName = "C:/UnrealEngine/Engine/Plugins/Runtime/Plugin2/Content/Plugin2",
        BuiltOn = DateTime.Now
    };
    _pluginManagementService.Setup(x => x.FindLocalPlugin("Plugin2", new SemVersion(1, 1, 0), "5.5", new List<string> {
            "Win64"
        }))
        .ReturnsAsync(plugin2);
    _pluginManagementService.Setup(x => x.DownloadPlugin("Plugin3", new SemVersion(1, 3, 0), null, "5.5",
            new List<string> {
                "Win64"
            }))
        .ReturnsAsync(plugin3);

    var changes = await _installService.InstallRequirements("Project.uproject", "5.5", ["Win64"]);
    Assert.That(changes, Has.Count.EqualTo(2));
    Assert.That(changes, Has.Member(new VersionChange("Plugin2", new SemVersion(1, 0, 0), new SemVersion(1, 1, 0))));
    Assert.That(changes, Has.Member(new VersionChange("Plugin3", new SemVersion(1, 3, 0), new SemVersion(1, 3, 0))));
  }
}