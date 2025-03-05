using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Compression;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Resolution;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Tests.Database;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Server.Config;
using UnrealPluginManager.Server.Controllers;
using UnrealPluginManager.Server.Services;
using UnrealPluginManager.Server.Utils;

namespace UnrealPluginManager.Server.Tests.Controllers;

public class PluginControllerTest {
  private static readonly JsonSerializerOptions JsonOptions = new() {
      AllowTrailingCommas = true
  };

  private ServiceProvider _serviceProvider;
  private PluginsController _pluginsController;

  [SetUp]
  public void Setup() {
    var services = new ServiceCollection();

    var mockFilesystem = new MockFileSystem(new Dictionary<string, MockFileData>());
    services.AddSingleton<IFileSystem>(mockFilesystem);

    var mockConfig = new Mock<IConfiguration>();
    services.AddSingleton(mockConfig.Object);
    var mockSection = new Mock<IConfigurationSection>();

    mockConfig.Setup(x => x.GetSection(StorageMetadata.Name)).Returns(mockSection.Object);

    services.AddDbContext<UnrealPluginManagerContext, TestUnrealPluginManagerContext>()
        .AddCoreServices()
        .AddServerServices();
    _serviceProvider = services.BuildServiceProvider();

    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    _pluginsController = new PluginsController(pluginService);
  }

  [TearDown]
  public void TearDown() {
    _serviceProvider.Dispose();
  }

  [Test]
  public async Task TestBasicAddAndGet() {
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    await pluginService.AddPlugin("Plugin1", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(1, 0, 0)
    }, null);

    await pluginService.AddPlugin("Plugin2", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(1, 0, 0),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "Plugin1",
                PluginType = PluginType.Provided,
                VersionMatcher = SemVersionRange.Parse(">=1.0.0")
            },
            new PluginReferenceDescriptor {
                Name = "Paper2D",
                PluginType = PluginType.Engine,
                VersionMatcher = SemVersionRange.Parse(">=1.0.0")
            }
        ]
    }, null);

    await pluginService.AddPlugin("Plugin3", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(1, 0, 0),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "Plugin2",
                PluginType = PluginType.Provided
            }
        ]
    }, null);

    await pluginService.AddPlugin("Plugin3", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(1, 2, 1),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "Plugin2",
                PluginType = PluginType.Provided
            }
        ]
    }, null);

    await pluginService.AddPlugin("Plugin4", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(1, 2, 1),
        Plugins = []
    }, null);

    var plugin1Result = await _pluginsController.GetDependencyTree("Plugin1");
    Assert.That(plugin1Result, Is.InstanceOf<ResolvedDependencies>());
    var plugin1List = ((ResolvedDependencies)plugin1Result).SelectedPlugins;
    Assert.That(plugin1List, Has.Count.EqualTo(1));
    Assert.That(plugin1List[0].Name, Is.EqualTo("Plugin1"));

    var plugin2Result = await _pluginsController.GetDependencyTree("Plugin2");
    Assert.That(plugin2Result, Is.InstanceOf<ResolvedDependencies>());
    var plugin2List = ((ResolvedDependencies)plugin2Result).SelectedPlugins;
    Assert.That(plugin2List, Has.Count.EqualTo(2));
    var plugin2Names = plugin2List.Select(x => x.Name).ToList();
    Assert.That(plugin2Names, Does.Contain("Plugin1"));
    Assert.That(plugin2Names, Does.Contain("Plugin2"));

    var plugin3Result = await _pluginsController.GetDependencyTree("Plugin3");
    Assert.That(plugin3Result, Is.InstanceOf<ResolvedDependencies>());
    var plugin3List = ((ResolvedDependencies)plugin3Result).SelectedPlugins;
    Assert.That(plugin3List, Has.Count.EqualTo(3));
    var plugin3Names = plugin3List.Select(x => x.Name).ToList();
    Assert.That(plugin3Names, Does.Contain("Plugin1"));
    Assert.That(plugin3Names, Does.Contain("Plugin2"));
    Assert.That(plugin3Names, Does.Contain("Plugin3"));

    var allPluginsList = await _pluginsController.GetPlugins();
    Assert.That(allPluginsList, Has.Count.EqualTo(4));
  }

  [Test]
  public async Task TestAddAndDownloadPlugin() {
    var filesystem = _serviceProvider.GetRequiredService<IFileSystem>();
    var tempFileName = Path.GetTempFileName();
    var dirName = Path.GetDirectoryName(tempFileName);
    Assert.That(dirName, Is.Not.Null);
    filesystem.Directory.CreateDirectory(dirName);

    using var testZip = new MemoryStream();
    using (var zipArchive = new ZipArchive(testZip, ZipArchiveMode.Create, true)) {
      zipArchive.CreateEntry("Resources/");
      zipArchive.CreateEntry("Resources/Icon128.png");
      zipArchive.CreateEntry("Binaries/");
      zipArchive.CreateEntry("Binaries/Win64/");
      zipArchive.CreateEntry("Binaries/Win64/TestPlugin.dll");

      var entry = zipArchive.CreateEntry("TestPlugin.uplugin");
      await using var writer = new StreamWriter(entry.Open());

      var descriptor = new PluginDescriptor {
          FriendlyName = "Test Plugin",
          VersionName = new SemVersion(1, 0, 0),
          Description = "Test description"
      };

      await writer.WriteAsync(JsonSerializer.Serialize(descriptor, JsonOptions));
    }

    testZip.Seek(0, SeekOrigin.Begin);
    var result = await _pluginsController.AddPlugin(new FormFile(testZip, 0, testZip.Length, "file", "TestPlugin.zip"),
                                                    new Version(5, 5));
    Assert.That(result.Name, Is.EqualTo("TestPlugin"));

    var downloaded = await _pluginsController.DownloadLatestPlugin("TestPlugin", new Version(5, 5), SemVersionRange.AllRelease, ["Win64"]);
    Assert.Multiple(() => {
      Assert.That(downloaded.FileDownloadName, Is.EqualTo("TestPlugin.zip"));
      Assert.That(downloaded.ContentType, Is.EqualTo("application/zip"));
    });
  }
}