using System.IO.Compression;
using Microsoft.Extensions.DependencyInjection;
using Semver;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Server.Tests.Helpers;
using UnrealPluginManager.WebClient.Api;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Server.Tests.Controllers;

public class PluginControllerTest {
  private TestWebApplicationFactory<Program> _factory;
  private HttpClient _client;
  private PluginsApi _pluginsApi;
  private IJsonService _jsonService;
  private IServiceProvider _serviceProvider;


  [SetUp]
  public void Setup() {
    _factory = new TestWebApplicationFactory<Program>();
    _client = _factory.CreateClient();
    _pluginsApi = new PluginsApi(_client);
    _serviceProvider = _factory.Services;
    _serviceProvider.GetRequiredService<TestCloudUnrealPluginManagerContext>()
        .Database.EnsureCreated();
    _serviceProvider.GetRequiredService<TestCloudUnrealPluginManagerContext.DeferredDelete>();
    _jsonService = _serviceProvider.GetRequiredService<IJsonService>();
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
    var pluginService = scope.ServiceProvider.GetRequiredService<IPluginService>();
    var plugin1 = await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Plugin1",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Plugin1/v1.0.0"),
            Sha = ""
        },
        Dependencies = []
    }, [], null, null);

    var plugin2 = await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Plugin2",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Plugin2/v1.0.0"),
            Sha = ""
        },
        Dependencies = [
            new PluginDependencyManifest() {
                Name = "Plugin1",
                Version = SemVersionRange.Parse(">=1.0.0")
            }
        ]
    }, []);

    var plugin3 = await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Plugin3",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Plugin3/v1.0.0"),
            Sha = ""
        },
        Dependencies = [
            new PluginDependencyManifest {
                Name = "Plugin2",
                Version = SemVersionRange.AtLeast(new SemVersion(1, 0, 0))
            }
        ]
    }, []);

    var plugin4 = await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Plugin3",
        Version = new SemVersion(1, 2, 1),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Plugin3/v1.2.1"),
            Sha = ""
        },
        Dependencies = [
            new PluginDependencyManifest {
                Name = "Plugin2",
                Version = SemVersionRange.AtLeast(new SemVersion(1, 0, 0))
            }
        ]
    }, []);

    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Plugin4",
        Version = new SemVersion(1, 2, 1),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Plugin4/v1.2.1"),
            Sha = ""
        },
        Dependencies = []
    }, []);

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
  public async Task TestAddReadme() {
    var manifestJson = _jsonService.Serialize(new PluginManifest {
        Name = "TestPlugin",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/TestPlugin/v1.0.0"),
            Sha = ""
        },
        Dependencies = []
    });
    using var memoryStream = new MemoryStream();
    using (var zipArchive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true)) {
      var entry = zipArchive.CreateEntry("plugin.json");
      await using var writer = entry.Open();
      await using var jsonStream = manifestJson.ToStream();
      await jsonStream.CopyToAsync(writer);
    }
    memoryStream.Position = 0;

    var result = await _pluginsApi.SubmitPluginAsync(new FileParameter(memoryStream));
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
}