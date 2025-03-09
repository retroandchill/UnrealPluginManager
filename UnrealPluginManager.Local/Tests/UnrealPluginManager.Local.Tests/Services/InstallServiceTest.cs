using System.CommandLine;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Semver;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Mappers;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Project;
using UnrealPluginManager.Core.Model.Resolution;
using UnrealPluginManager.Core.Services;
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
    _engineService.Setup(x => x.GetInstalledPluginVersion("TestPlugin", "5.5"))
        .ReturnsAsync(new SemVersion(1, 0, 0));

    var result = await _installService.InstallPlugin("TestPlugin", SemVersionRange.All, "5.5", ["Win64"]);
    Assert.That(result, Has.Count.EqualTo(0));

    var existingPlugin = new PluginVersionInfo {
        PluginId = 1,
        Name = "TestPlugin",
        FriendlyName = "Test Plugin",
        VersionId = 1,
        Version = new SemVersion(1, 1, 0),
        Dependencies = []
    };

    var targetVersion = SemVersionRange.AtLeast(new SemVersion(1, 1, 0));
    _pluginManagementService.Setup(x => x.FindTargetPlugin("TestPlugin", targetVersion, "5.5"))
        .ReturnsAsync(existingPlugin);

    _pluginManagementService.Setup(x => x.GetPluginsToInstall(existingPlugin, "5.5"))
        .ReturnsAsync(new List<PluginSummary> {
            existingPlugin.ToPluginSummary()
        });

    _engineService.Setup(x => x.GetInstalledPlugins("5.5"))
        .Returns((new List<InstalledPlugin> {
            new("TestPlugin", new SemVersion(1, 0, 0), ["Win64"])
        }).ToAsyncEnumerable());

    var changes = await _installService.InstallPlugin("TestPlugin", targetVersion, "5.5", ["Win64"]);
    Assert.That(changes, Has.Count.EqualTo(1));
    Assert.Multiple(() => {
      Assert.That(changes[0].PluginName, Is.EqualTo("TestPlugin"));
      Assert.That(changes[0].OldVersion, Is.EqualTo(new SemVersion(1, 0, 0)));
      Assert.That(changes[0].NewVersion, Is.EqualTo(new SemVersion(1, 1, 0)));
    });

    _engineService.Verify(x => x.InstallPlugin("TestPlugin", new SemVersion(1, 1, 0), "5.5", new List<string> {
        "Win64"
    }), Times.Once());
  }

  [Test]
  public async Task TestInstallPlugin_HasConflicts() {
    var existingPlugin = new PluginVersionInfo {
        PluginId = 1,
        Name = "TestPlugin",
        FriendlyName = "Test Plugin",
        VersionId = 1,
        Version = new SemVersion(1, 1, 0),
        Dependencies = []
    };

    var targetVersion = SemVersionRange.AtLeast(new SemVersion(1, 1, 0));
    _pluginManagementService.Setup(x => x.FindTargetPlugin("TestPlugin", targetVersion, "5.5"))
        .ReturnsAsync(existingPlugin);

    _pluginManagementService.Setup(x => x.GetPluginsToInstall(existingPlugin, "5.5"))
        .ThrowsAsync(new DependencyConflictException(new List<Conflict> {
            new("DependentPlugin", [
                new PluginRequirement("TestPlugin", SemVersionRange.AtLeast(new SemVersion(2, 0, 0))),
                new PluginRequirement("OtherPlugin", SemVersionRange.LessThan(new SemVersion(2, 0, 0)))
            ])
        }));

    var conflicts = Assert.ThrowsAsync<DependencyConflictException>(async () => await _installService.InstallPlugin("TestPlugin", targetVersion, "5.5", ["Win64"]));
    Assert.That(conflicts.Conflicts, Has.Count.EqualTo(1));
    _engineService.Verify(x => x.InstallPlugin("TestPlugin", new SemVersion(1, 1, 0), "5.5", new List<string> {
        "Win64"
    }), Times.Never());
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
                PluginId = 1,
                Name = "Plugin1",
                FriendlyName = "Plugin 1",
                VersionId = 1,
                Version = new SemVersion(1, 0, 0),
            },
            new() {
                PluginId = 2,
                Name = "Plugin2",
                FriendlyName = "Plugin 2",
                VersionId = 2,
                Version = new SemVersion(1, 1, 0),
            },
            new() {
                PluginId = 2,
                Name = "Plugin3",
                FriendlyName = "Plugin 3",
                VersionId = 2,
                Version = new SemVersion(1, 3, 0),
            }
        });

    var installedPlugins = new List<InstalledPlugin> {
        new("Plugin1", new SemVersion(1, 0, 0), ["Win64"]),
        new("Plugin2", new SemVersion(1, 0, 0), ["Win64"]),
        new("Plugin3", new SemVersion(1, 3, 0), []),
    };
    _engineService.Setup(x => x.GetInstalledPlugins("5.5"))
        .Returns(installedPlugins.ToAsyncEnumerable());
    
    var changes = await _installService.InstallRequirements("Project.uproject", "5.5", ["Win64"]);
    Assert.That(changes, Has.Count.EqualTo(2));
    Assert.That(changes, Has.Member(new VersionChange("Plugin2", new SemVersion(1, 0, 0), new SemVersion(1, 1, 0))));
    Assert.That(changes, Has.Member(new VersionChange("Plugin3", new SemVersion(1, 3, 0), new SemVersion(1, 3, 0))));
    
    _engineService.Verify(x => x.InstallPlugin("Plugin2", new SemVersion(1, 1, 0), "5.5", new List<string> { "Win64" }), Times.Once());
    _engineService.Verify(x => x.InstallPlugin("Plugin3", new SemVersion(1, 3, 0), "5.5", new List<string> { "Win64" }), Times.Once());
  }
}