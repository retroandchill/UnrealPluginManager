using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Tests.Mocks;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Local.Services;
using UnrealPluginManager.Local.Tests.Mocks;

namespace UnrealPluginManager.Local.Tests.Services;

public class RemoteServiceTest {
  private static readonly string UserPath = Path.GetFullPath("C:/Users/TestUser");
  private static readonly string ConfigPath = Path.Join(UserPath, ".UnrealPluginManager", "Config");

  private ServiceProvider _serviceProvider;
  private MockFileSystem _filesystem;
  private MockEnvironment _environment;
  private IRemoteService _remoteService;

  [SetUp]
  public void Setup() {
    var services = new ServiceCollection();

    _filesystem = new MockFileSystem(new Dictionary<string, MockFileData>());
    _filesystem.Directory.CreateDirectory(ConfigPath);
    services.AddSingleton<IFileSystem>(_filesystem);

    _environment = new MockEnvironment {
        SpecialFolders = {
            [Environment.SpecialFolder.UserProfile] = UserPath
        }
    };
    services.AddSingleton<IEnvironment>(_environment);
    
    services.AddSingleton<IJsonService>(new JsonService(JsonSerializerOptions.Default));
    services.AddSingleton<IApiTypeResolver, MockTypeResolver>();
    services.AddSingleton<IStorageService, LocalStorageService>();
    services.AddScoped<IRemoteService, RemoteService>();
    _serviceProvider = services.BuildServiceProvider();
    _remoteService = _serviceProvider.GetRequiredService<IRemoteService>();
  }

  [TearDown]
  public void TearDown() {
    _serviceProvider.Dispose();
  }

  [Test]
  public void TestConfigureRemotes() {
    var defaultRemote = _remoteService.GetRemote("default");
    Assert.That(defaultRemote.IsSome, Is.True);
    var defaultRemoteValue = defaultRemote.OrElseThrow().Url;

    var invalidRemote = _remoteService.GetRemote("invalid");
    Assert.That(invalidRemote.IsNone, Is.True);

    Assert.ThrowsAsync<ArgumentException>(
        () => _remoteService.AddRemote("default", new Uri("https://unrealpluginmanager.com")));
    Assert.DoesNotThrowAsync(
        () => _remoteService.AddRemote(
            "alt", new Uri("https://github.com/api/v1/repos/EpicGames/UnrealEngine/releases/latest")));

    var allRemotes = _remoteService.GetAllRemotes()
        .ToDictionary(x => x.Key, x => x.Value.Url);
    Assert.That(allRemotes, Has.Count.EqualTo(2));
    Assert.That(allRemotes, Does.ContainKey("default").WithValue(defaultRemoteValue));
    Assert.That(
        allRemotes,
        Does.ContainKey("alt")
            .WithValue(new Uri("https://github.com/api/v1/repos/EpicGames/UnrealEngine/releases/latest")));

    Assert.ThrowsAsync<ArgumentException>(() => _remoteService.RemoveRemote("invalid"));
    Assert.DoesNotThrowAsync(() => _remoteService.RemoveRemote("alt"));

    var altRemote = _remoteService.GetRemote("alt");
    Assert.That(altRemote.IsNone, Is.True);

    Assert.ThrowsAsync<ArgumentException>(
        () => _remoteService.UpdateRemote("alt", new Uri("https://unrealpluginmanager.com")));
    Assert.DoesNotThrowAsync(() => _remoteService.UpdateRemote("default", new Uri("https://unrealpluginmanager.com")));

    allRemotes = _remoteService.GetAllRemotes()
        .ToDictionary(x => x.Key, x => x.Value.Url);
    Assert.That(allRemotes, Has.Count.EqualTo(1));
    Assert.That(allRemotes, Does.ContainKey("default").WithValue(new Uri("https://unrealpluginmanager.com")));
  }
}