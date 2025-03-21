﻿using System.IO.Abstractions;
using System.IO.Compression;
using Microsoft.Extensions.DependencyInjection;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Storage;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Tests.Helpers;
using UnrealPluginManager.WebClient.Api;
using UnrealPluginManager.WebClient.Client;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace UnrealPluginManager.Server.Tests.Controllers;

public class PluginControllerTest {
  private TestWebApplicationFactory<Program> _factory;
  private HttpClient _client;
  private PluginsApi _pluginsApi;
  private IServiceProvider _serviceProvider;


  [SetUp]
  public void Setup() {
    _factory = new TestWebApplicationFactory<Program>();
    _client = _factory.CreateClient();
    _pluginsApi = new PluginsApi(_client);
    _serviceProvider = _factory.Services;
    _serviceProvider.GetRequiredService<UnrealPluginManagerContext>()
        .Database.EnsureCreated();
  }

  [TearDown]
  public void TearDown() {
    _client.Dispose();
    _factory.Dispose();
    _pluginsApi.Dispose();
  }

  [Test]
  public async Task TestBasicAddAndGet() {
    using var scope = _serviceProvider.CreateScope();
    var dummy = _serviceProvider.GetRequiredService<IFileSystem>().FileInfo.New("Dummy");
    var partitioned = new PartitionedPlugin(new ResourceHandle("Source", dummy), null, null, []);
    var pluginService = scope.ServiceProvider.GetRequiredService<IPluginService>();
    var plugin1 = await pluginService.AddPlugin("Plugin1", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(1, 0, 0)
    }, partitioned);

    var plugin2 = await pluginService.AddPlugin("Plugin2", new PluginDescriptor {
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
    }, partitioned);

    var plugin3 = await pluginService.AddPlugin("Plugin3", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(1, 0, 0),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "Plugin2",
                PluginType = PluginType.Provided
            }
        ]
    }, partitioned);

    var plugin4 = await pluginService.AddPlugin("Plugin3", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(1, 2, 1),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "Plugin2",
                PluginType = PluginType.Provided
            }
        ]
    }, partitioned);

    await pluginService.AddPlugin("Plugin4", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(1, 2, 1),
        Plugins = []
    }, partitioned);

    var plugin1List = await _pluginsApi.GetDependencyTreeAsync(plugin1.PluginId);
    Assert.That(plugin1List, Has.Count.EqualTo(1));
    Assert.That(plugin1List[0].Name, Is.EqualTo("Plugin1"));

    var plugin2List = await _pluginsApi.GetDependencyTreeAsync(plugin2.PluginId);
    Assert.That(plugin2List, Has.Count.EqualTo(2));
    var plugin2Names = plugin2List.Select(x => x.Name).ToList();
    Assert.That(plugin2Names, Does.Contain("Plugin1"));
    Assert.That(plugin2Names, Does.Contain("Plugin2"));

    var plugin3List = await _pluginsApi.GetDependencyTreeAsync(plugin3.PluginId);
    Assert.That(plugin3List, Has.Count.EqualTo(3));
    var plugin3Names = plugin3List.Select(x => x.Name).ToList();
    Assert.That(plugin3Names, Does.Contain("Plugin1"));
    Assert.That(plugin3Names, Does.Contain("Plugin2"));
    Assert.That(plugin3Names, Does.Contain("Plugin3"));

    var allPluginsList = await _pluginsApi.GetPluginsAsync();
    Assert.That(allPluginsList, Has.Count.EqualTo(4));

    var plugin3Latest = await _pluginsApi.GetLatestVersionAsync(plugin3.PluginId);
    Assert.Multiple(() => {
      Assert.That(plugin3Latest.Name, Is.EqualTo("Plugin3"));
      Assert.That(plugin3Latest.Version, Is.EqualTo(new SemVersion(1, 2, 1)));
    });

    var plugin3Constrained =
        await _pluginsApi.GetLatestVersionAsync(plugin3.PluginId, SemVersionRange.Parse("<1.2.0").ToString());
    Assert.Multiple(() => {
      Assert.That(plugin3Constrained.Name, Is.EqualTo("Plugin3"));
      Assert.That(plugin3Constrained.Version, Is.EqualTo(new SemVersion(1, 0, 0)));
    });

    var latest = await _pluginsApi.GetLatestVersionsAsync();
    Assert.That(latest, Has.Count.EqualTo(4));
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

      await writer.WriteAsync(JsonSerializer.Serialize(descriptor));
    }

    testZip.Seek(0, SeekOrigin.Begin);
    var result = await _pluginsApi.AddPluginAsync("5.5", testZip);
    Assert.That(result.Name, Is.EqualTo("TestPlugin"));

    var pluginId = result.PluginId;
    var version = result.VersionId;

    Assert.DoesNotThrowAsync(() => _pluginsApi.DownloadPluginSourceAsync(pluginId, version));
    Assert.ThrowsAsync<ApiException>(() => _pluginsApi.DownloadPluginSourceAsync(pluginId, Guid.NewGuid()));
    Assert.DoesNotThrowAsync(() => _pluginsApi.DownloadPluginBinariesAsync(pluginId, version, "5.5", "Win64"));
    Assert.ThrowsAsync<ApiException>(() =>
        _pluginsApi.DownloadPluginBinariesAsync(pluginId, version, "5.5", "Linux"));
    Assert.ThrowsAsync<ApiException>(() =>
        _pluginsApi.DownloadPluginBinariesAsync(pluginId, Guid.NewGuid(), "5.5", "Win64"));
    Assert.DoesNotThrowAsync(() => _pluginsApi.DownloadPluginVersionAsync(pluginId, version, "5.5", ["Win64"]));
    Assert.DoesNotThrowAsync(() =>
        _pluginsApi.DownloadLatestPluginAsync(pluginId, "5.5", SemVersionRange.AllRelease.ToString(), ["Win64"]));
  }

  [Test]
  public async Task TestAddReadme() {
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

      await writer.WriteAsync(JsonSerializer.Serialize(descriptor));
    }

    testZip.Seek(0, SeekOrigin.Begin);
    var result = await _pluginsApi.AddPluginAsync("5.5", testZip);
    Assert.That(result.Name, Is.EqualTo("TestPlugin"));

    var pluginId = result.PluginId;
    var version = result.VersionId;

    Assert.ThrowsAsync<ApiException>(() => _pluginsApi.GetPluginReadmeAsync(pluginId, version));

    const string readme = "Test readme";
    await _pluginsApi.AddPluginReadmeAsync(pluginId, version, readme);
    var readmeResult = await _pluginsApi.GetPluginReadmeAsync(pluginId, version);
    Assert.That(readmeResult, Is.EqualTo(readme));

    const string readme2 = "Test readme 2";
    await _pluginsApi.UpdatePluginReadmeAsync(pluginId, version, readme2);
    var readmeResult2 = await _pluginsApi.GetPluginReadmeAsync(pluginId, version);
    Assert.That(readmeResult2, Is.EqualTo(readme2));
  }

  [Test]
  public async Task TestUploadPluginInParts() {
    var filesystem = _serviceProvider.GetRequiredService<IFileSystem>();
    var tempFileName = Path.GetTempFileName();
    var dirName = Path.GetDirectoryName(tempFileName);
    Assert.That(dirName, Is.Not.Null);
    filesystem.Directory.CreateDirectory(dirName);

    using var testZip = new MemoryStream();
    using (var zipArchive = new ZipArchive(testZip, ZipArchiveMode.Create, true)) {
      zipArchive.CreateEntry("Icon.png");
      zipArchive.CreateEntry("Binaries/");
      zipArchive.CreateEntry("Binaries/5.5/");
      zipArchive.CreateEntry("Binaries/5.5/Win64.zip");

      using var innerZip = new MemoryStream();
      using (var innerArchive = new ZipArchive(innerZip, ZipArchiveMode.Create, true)) {
        var entry = innerArchive.CreateEntry("TestPlugin.uplugin");
        await using var writer = new StreamWriter(entry.Open());

        var descriptor = new PluginDescriptor {
            FriendlyName = "Test Plugin",
            VersionName = new SemVersion(1, 0, 0),
            Description = "Test description"
        };

        await writer.WriteAsync(JsonSerializer.Serialize(descriptor));
      }

      var sourceEntry = zipArchive.CreateEntry("Source.zip");
      await using var sourceWriter = sourceEntry.Open();
      innerZip.Seek(0, SeekOrigin.Begin);
      await innerZip.CopyToAsync(sourceWriter);
    }

    testZip.Seek(0, SeekOrigin.Begin);
    var result = await _pluginsApi.SubmitPluginAsync(testZip);
    Assert.That(result.Name, Is.EqualTo("TestPlugin"));
  }
}