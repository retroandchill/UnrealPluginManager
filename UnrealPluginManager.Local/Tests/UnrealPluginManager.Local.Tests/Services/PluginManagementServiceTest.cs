using System.CommandLine;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Retro.SimplePage;
using Semver;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Core.Model.Storage;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Local.Config;
using UnrealPluginManager.Local.Exceptions;
using UnrealPluginManager.Local.Factories;
using UnrealPluginManager.Local.Model.Cache;
using UnrealPluginManager.Local.Model.Engine;
using UnrealPluginManager.Local.Model.Plugins;
using UnrealPluginManager.Local.Services;
using UnrealPluginManager.WebClient.Api;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Local.Tests.Services;

public class PluginManagementServiceTest {
  private ServiceProvider _serviceProvider;
  private MockFileSystem _filesystem;
  private Mock<IStorageService> _storageService;
  private Mock<IPluginsApi> _pluginsApi;
  private Mock<IPluginService> _pluginService;
  private Mock<IEngineService> _engineService;
  private Mock<ISourceDownloadService> _sourceDownloadService;
  private Mock<IBinaryCacheService> _binaryCacheService;
  private OrderedDictionary<string, RemoteConfig> _remoteConfigs;
  private IPluginManagementService _pluginManagementService;

  [SetUp]
  public void Setup() {
    var services = new ServiceCollection();

    _storageService = new Mock<IStorageService>();
    services.AddSingleton(_storageService.Object);
    _remoteConfigs = new OrderedDictionary<string, RemoteConfig> {
        ["default"] = new() {
            Url = new Uri("https://unrealpluginmanager.com")
        },
        ["alt"] = new() {
            Url = new Uri("https://github.com/api/v1/repos/EpicGames/UnrealEngine/releases/latest")
        },
        ["unaccessible"] = new() {
            Url = new Uri("https://unrealpluginmanager.com/invalid")
        }
    };
    _storageService.Setup(x => x.GetConfig(It.IsAny<string>(), It.IsAny<OrderedDictionary<string, RemoteConfig>>()))
        .Returns(_remoteConfigs);

    _pluginService = new Mock<IPluginService>();
    services.AddSingleton(_pluginService.Object);

    services.AddSingleton<IPluginStructureService, PluginStructureService>();

    _engineService = new Mock<IEngineService>();
    services.AddSingleton(_engineService.Object);

    _pluginsApi = new Mock<IPluginsApi>();
    services.AddSingleton(_pluginsApi.Object);
    services.AddSingleton<IApiAccessor>(_pluginsApi.Object);

    _filesystem = new MockFileSystem(new Dictionary<string, MockFileData>());
    services.AddSingleton<IFileSystem>(_filesystem);

    _sourceDownloadService = new Mock<ISourceDownloadService>();
    services.AddSingleton(_sourceDownloadService.Object);

    _binaryCacheService = new Mock<IBinaryCacheService>();
    services.AddSingleton(_binaryCacheService.Object);

    var mockClientFactory = new Mock<IApiClientFactory>();
    mockClientFactory.SetupGet(x => x.InterfaceType).Returns(typeof(IPluginsApi));
    mockClientFactory.Setup(x => x.Create(It.IsAny<RemoteConfig>()))
        .Returns(_pluginsApi.Object);
    services.AddSingleton(mockClientFactory.Object);

    var console = new Mock<IConsole>();
    services.AddSingleton(console.Object);

    services.AddSingleton(_pluginsApi.Object);
    services.AddScoped<IRemoteService, RemoteService>();
    services.AddScoped<IPluginManagementService, PluginManagementService>();
    var jsonService = new JsonService(JsonSerializerOptions.Default);
    services.AddSingleton<IJsonService>(jsonService);

    _serviceProvider = services.BuildServiceProvider();

    var realPluginService = new PluginService(null, null, null, null, null);
    _pluginService.Setup(x => x.GetDependencyList(It.IsAny<IDependencyChainNode>(), It.IsAny<DependencyManifest>()))
        .Returns((IDependencyChainNode root, DependencyManifest manifest) =>
                     realPluginService.GetDependencyList(root, manifest));

    _pluginManagementService = _serviceProvider.GetRequiredService<IPluginManagementService>();

    var pageList = AddPluginsToRemote(300)
        .Concat(AddPluginsToRemote(50))
        .ToList();

    var pageIndex = 0;
    _pluginsApi.Setup(x => x.GetPluginsAsync(It.IsAny<string>(), It.IsAny<int?>(),
                                             It.Is(100, EqualityComparer<int>.Default), It.IsAny<CancellationToken>()))
        .Returns((string? _, int? _, int? _, CancellationToken _) => {
          if (pageIndex >= pageList.Count) {
            throw new ApiException(404, "Unreachable");
          }

          return Task.FromResult(pageList[pageIndex++]);
        });
  }

  [TearDown]
  public void TearDown() {
    _serviceProvider.Dispose();
  }

  private static List<Page<PluginOverview>> AddPluginsToRemote(int totalCount) {
    return Enumerable.Range(0, totalCount)
        .Select(i => new PluginOverview {
            Id = Guid.NewGuid(),
            Name = $"Plugin{i + 1}",
            Versions = []
        })
        .AsPages(100)
        .ToList();
  }

  [Test]
  public async Task TestGetPluginsFromAllRemotes() {
    var plugins = await _pluginManagementService.GetPlugins("*");
    Assert.That(plugins, Has.Count.EqualTo(3));
    Assert.That(plugins, Does.ContainKey("default"));
    Assert.That(plugins["default"].IsSucc, Is.True);
    Assert.That(plugins["default"].OrElseThrow(), Has.Count.EqualTo(300));

    Assert.That(plugins, Does.ContainKey("alt"));
    Assert.That(plugins["alt"].IsSucc, Is.True);
    Assert.That(plugins["alt"].OrElseThrow(), Has.Count.EqualTo(50));

    Assert.That(plugins, Does.ContainKey("unaccessible"));
    Assert.That(plugins["unaccessible"].IsFail, Is.True);
  }

  [Test]
  public async Task TestGetPluginsFromSingleRemote() {
    var plugins = await _pluginManagementService.GetPlugins("default", "*");
    Assert.That(plugins, Has.Count.EqualTo(300));

    plugins = await _pluginManagementService.GetPlugins("alt", "*");
    Assert.That(plugins, Has.Count.EqualTo(50));

    Assert.ThrowsAsync<ArgumentException>(() => _pluginManagementService.GetPlugins("invalid", "*"));
  }

  [Test]
  public async Task TestGetPluginDependencyTreet() {
    var root = new DependencyChainRoot {
        Dependencies = [
            new PluginDependency {
                PluginName = "Sql",
                PluginVersion = SemVersionRange.Parse("=2.0.0")
            },
            new PluginDependency {
                PluginName = "Threads",
                PluginVersion = SemVersionRange.Parse("=2.0.0")
            },
            new PluginDependency {
                PluginName = "Http",
                PluginVersion = SemVersionRange.Parse(">=3.0.0 <=4.0.0")
            },
            new PluginDependency {
                PluginName = "StdLib",
                PluginVersion = SemVersionRange.Parse("=4.0.0")
            }
        ]
    };

    _engineService.Setup(x => x.GetInstalledPlugins(null))
        .Returns(new List<InstalledPlugin> {
            new("StdLib", new SemVersion(4, 0, 0), ["Win64"]),
            new("Http", new SemVersion(3, 0, 0), ["Win64"]),
        }.ToAsyncEnumerable());

    var allPlugins = new Dictionary<string, List<SemVersion>> {
        ["Sql"] = [new SemVersion(0, 1, 0), new SemVersion(1, 0, 0), new SemVersion(2, 0, 0)],
        ["Threads"] = [new SemVersion(0, 1, 0), new SemVersion(1, 0, 0), new SemVersion(2, 0, 0)],
        ["Http"] = [
            new SemVersion(0, 1, 0), new SemVersion(1, 0, 0), new SemVersion(2, 0, 0), new SemVersion(3, 0, 0),
            new SemVersion(4, 0, 0)
        ],
        ["StdLib"] = [
            new SemVersion(0, 1, 0), new SemVersion(1, 0, 0), new SemVersion(2, 0, 0), new SemVersion(3, 0, 0),
            new SemVersion(4, 0, 0)
        ]
    };

    _pluginService.Setup(x => x.GetPossibleVersions(root.Dependencies))
        .Returns(Task.FromResult(new DependencyManifest {
            FoundDependencies = allPlugins.Where(x => x.Key is "Http" or "StdLib")
                .ToDictionary(x => x.Key, x => x.Value
                                  .Select(y =>
                                              new PluginVersionInfo {
                                                  VersionId = Guid.NewGuid(),
                                                  PluginId = Guid.NewGuid(),
                                                  Name = x.Key,
                                                  Version = y,
                                                  Dependencies = []
                                              })
                                  .ToList())
        }));

    _pluginsApi.Setup(x => x.GetCandidateDependenciesAsync(
                          It.IsAny<List<PluginDependency>>(), It.IsAny<CancellationToken>()))
        .Returns(Task.FromResult(new DependencyManifest {
            FoundDependencies = allPlugins.Where(x => x.Key is not "Http" and not "StdLib")
                .ToDictionary(x => x.Key, x => x.Value
                                  .Select(y =>
                                              new PluginVersionInfo {
                                                  VersionId = Guid.NewGuid(),
                                                  PluginId = Guid.NewGuid(),
                                                  Name = x.Key,
                                                  Version = y,
                                                  Dependencies = []
                                              })
                                  .ToList())
        }));

    var dependencyGraph = await _pluginManagementService.GetPluginsToInstall(root, null);
    Assert.That(dependencyGraph, Has.Count.EqualTo(4));
    Assert.Multiple(() => {
      Assert.That(dependencyGraph.Find(x => x.Name == "Threads")?.Version, Is.EqualTo(new SemVersion(2, 0, 0)));
      Assert.That(dependencyGraph.Find(x => x.Name == "StdLib")?.Version, Is.EqualTo(new SemVersion(4, 0, 0)));
      Assert.That(dependencyGraph.Find(x => x.Name == "Sql")?.Version, Is.EqualTo(new SemVersion(2, 0, 0)));
      Assert.That(dependencyGraph.Find(x => x.Name == "Http")?.Version, Is.EqualTo(new SemVersion(3, 0, 0)));
    });
  }

  [Test]
  public async Task TestFindTargetPlugin() {
    _engineService.Setup(x => x.GetInstalledPluginVersion("TestPlugin", "5.5"))
        .ReturnsAsync(LanguageExt.Option<SemVersion>.None);
    _pluginService.Setup(x => x.GetPluginVersionInfo("TestPlugin", SemVersionRange.All))
        .ReturnsAsync(LanguageExt.Option<PluginVersionInfo>.None);

    _pluginsApi.Setup(x =>
                          x.GetLatestVersionsAsync("TestPlugin", SemVersionRange.All.ToString(), 1, 100,
                                                   CancellationToken.None))
        .ReturnsAsync(new Page<PluginVersionInfo>([
            new PluginVersionInfo {
                PluginId = Guid.NewGuid(),
                Name = "TestPlugin",
                VersionId = Guid.NewGuid(),
                Version = new SemVersion(1, 1, 0),
                Dependencies = []
            }
        ]));

    var targetPlugin = await _pluginManagementService.FindTargetPlugin("TestPlugin", SemVersionRange.All, "5.5");
    Assert.That(targetPlugin, Is.Not.Null);
    Assert.Multiple(() => {
      Assert.That(targetPlugin.Name, Is.EqualTo("TestPlugin"));
      Assert.That(targetPlugin.Version, Is.EqualTo(new SemVersion(1, 1, 0)));
      Assert.That(targetPlugin.Dependencies, Is.Empty);
    });
  }

  [Test]
  public async Task TestUploadPlugin() {
    var version = new SemVersion(1, 0, 0);
    var versionMatcher = SemVersionRange.Equals(version);

    var pluginVersion = new PluginVersionDetails {
        PluginId = Guid.NewGuid(),
        Name = "TestPlugin",
        VersionId = Guid.NewGuid(),
        Version = version,
        Dependencies = []
    };

    _pluginService.Setup(x => x.ListLatestVersions("TestPlugin", versionMatcher, default))
        .ReturnsAsync(new Page<PluginVersionInfo>([pluginVersion]));
    _pluginService.Setup(x => x.GetSourcePatches(pluginVersion.PluginId, pluginVersion.VersionId))
        .ReturnsAsync([]);

    _pluginsApi.Setup(x => x.SubmitPluginAsync(It.IsAny<FileParameter>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(pluginVersion);

    var result = await _pluginManagementService.UploadPlugin("TestPlugin", version, "default");
    Assert.That(result.PluginId, Is.EqualTo(pluginVersion.PluginId));
  }

  [Test]
  public void TestUploadPlugin_NotThere() {
    var version = new SemVersion(1, 0, 0);
    var versionMatcher = SemVersionRange.Equals(version);

    _pluginService.Setup(x => x.ListLatestVersions("TestPlugin", versionMatcher, default))
        .ReturnsAsync(new Page<PluginVersionInfo>([]));

    Assert.ThrowsAsync<PluginNotFoundException>(() =>
                                                    _pluginManagementService.UploadPlugin(
                                                        "TestPlugin", version, "default"));
  }

  [Test]
  public async Task TestUploadPlugin_WithIcon() {
    var version = new SemVersion(1, 0, 0);
    var pluginVersion = new PluginVersionDetails {
        PluginId = Guid.NewGuid(),
        Name = "TestPlugin",
        VersionId = Guid.NewGuid(),
        Version = version,
        Dependencies = [],
        Icon = new ResourceInfo { StoredFilename = "icon.png", OriginalFilename = "icon.png" }
    };

    _pluginService.Setup(x => x.ListLatestVersions("TestPlugin", SemVersionRange.Equals(version), default))
        .ReturnsAsync(new Page<PluginVersionInfo>([pluginVersion]));
    _pluginService.Setup(x => x.GetSourcePatches(pluginVersion.PluginId, pluginVersion.VersionId))
        .ReturnsAsync([]);

    // Setup mock icon stream
    var iconStream = new MemoryStream(new byte[] { 1, 2, 3, 4 });
    _storageService.Setup(x => x.GetResourceStream("icon.png"))
        .Returns(iconStream);

    _pluginsApi.Setup(x => x.SubmitPluginAsync(It.IsAny<FileParameter>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(pluginVersion);

    var result = await _pluginManagementService.UploadPlugin("TestPlugin", version, "default");
    Assert.That(result.PluginId, Is.EqualTo(pluginVersion.PluginId));
  }

  [Test]
  public async Task TestUploadPlugin_WithReadme() {
    var version = new SemVersion(1, 0, 0);
    var pluginVersion = new PluginVersionDetails {
        PluginId = Guid.NewGuid(),
        Name = "TestPlugin",
        VersionId = Guid.NewGuid(),
        Version = version,
        Dependencies = []
    };

    _pluginService.Setup(x => x.ListLatestVersions("TestPlugin", SemVersionRange.Equals(version), default))
        .ReturnsAsync(new Page<PluginVersionInfo>([pluginVersion]));
    _pluginService.Setup(x => x.GetSourcePatches(pluginVersion.PluginId, pluginVersion.VersionId))
        .ReturnsAsync([]);

    // Setup mock readme content
    const string readmeContent = "# Test Plugin\nThis is a test plugin";
    _pluginService.Setup(x => x.GetPluginReadme(pluginVersion.PluginId, pluginVersion.VersionId))
        .ReturnsAsync(readmeContent);

    _pluginsApi.Setup(x => x.SubmitPluginAsync(It.IsAny<FileParameter>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(pluginVersion);

    var result = await _pluginManagementService.UploadPlugin("TestPlugin", version, "default");
    Assert.That(result.PluginId, Is.EqualTo(pluginVersion.PluginId));
  }

  [Test]
  public async Task TestUploadPlugin_WithPatches() {
    var version = new SemVersion(1, 0, 0);
    var pluginVersion = new PluginVersionDetails {
        PluginId = Guid.NewGuid(),
        Name = "TestPlugin",
        VersionId = Guid.NewGuid(),
        Version = version,
        Dependencies = [],
        Patches = ["patch1.diff", "patch2.diff"]
    };

    _pluginService.Setup(x => x.ListLatestVersions("TestPlugin", SemVersionRange.Equals(version), default))
        .ReturnsAsync(new Page<PluginVersionInfo>([pluginVersion]));

    // Setup mock patches
    var patches = new List<string> { "patch1.diff", "patch2.diff" };
    _pluginService.Setup(x => x.GetSourcePatches(pluginVersion.PluginId, pluginVersion.VersionId))
        .ReturnsAsync(patches.Select(p => new SourcePatchInfo(p, $"Content of {p}")).ToList());

    _pluginsApi.Setup(x => x.SubmitPluginAsync(It.IsAny<FileParameter>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(pluginVersion);

    var result = await _pluginManagementService.UploadPlugin("TestPlugin", version, "default");
    Assert.That(result.PluginId, Is.EqualTo(pluginVersion.PluginId));
  }

  [Test]
  public void TestUploadPlugin_PatchesMismatch() {
    var version = new SemVersion(1, 0, 0);
    var pluginVersion = new PluginVersionDetails {
        PluginId = Guid.NewGuid(),
        Name = "TestPlugin",
        VersionId = Guid.NewGuid(),
        Version = version,
        Dependencies = []
    };

    _pluginService.Setup(x => x.ListLatestVersions("TestPlugin", SemVersionRange.Equals(version), default))
        .ReturnsAsync(new Page<PluginVersionInfo>([pluginVersion]));

    // Setup patches with mismatched count
    var patches = new List<string> { "patch1.diff" }; // Only one patch
    _pluginService.Setup(x => x.GetSourcePatches(pluginVersion.PluginId, pluginVersion.VersionId))
        .ReturnsAsync(patches.Select(p => new SourcePatchInfo(p, $"Content of {p}")).ToList());

    // The manifest expects two patches but we only provide one
    pluginVersion.Patches = new List<string> { "patch1.diff", "patch2.diff" };

    Assert.ThrowsAsync<BadSubmissionException>(() =>
                                                   _pluginManagementService.UploadPlugin(
                                                       "TestPlugin", version, "default"));
  }

  [Test]
  public void TestUploadPlugin_NoDefaultRemote() {
    var version = new SemVersion(1, 0, 0);
    var pluginVersion = new PluginVersionDetails {
        PluginId = Guid.NewGuid(),
        Name = "TestPlugin",
        VersionId = Guid.NewGuid(),
        Version = version,
        Dependencies = []
    };

    _pluginService.Setup(x => x.ListLatestVersions("TestPlugin", SemVersionRange.Equals(version), default))
        .ReturnsAsync(new Page<PluginVersionInfo>([pluginVersion]));

    _remoteConfigs.Clear();

    // Test when no remote is specified and no default remote exists
    Assert.ThrowsAsync<RemoteNotFoundException>(() =>
                                                    _pluginManagementService.UploadPlugin("TestPlugin", version, null));
  }


  private static readonly IReadOnlyCollection<string> Platforms = ["Win64"];

  [Test]
  public async Task TestDownloadPlugin_AlreadyDownloaded() {
    _binaryCacheService.Setup(x => x.GetCachedPluginBuild("TestPlugin", new SemVersion(1, 0, 0), "5.5", Platforms))
        .ReturnsAsync(new PluginBuildInfo {
            PluginName = "TestPlugin",
            PluginVersion = new SemVersion(1, 0, 0),
            BuiltOn = DateTimeOffset.Now,
            DirectoryName = "",
            EngineVersion = "5.5"
        });
    var result =
        await _pluginManagementService.DownloadPlugin("TestPlugin", new SemVersion(1, 0, 0), 0, "5.5", ["Win64"]);

    Assert.That(result.PluginName, Is.EqualTo("TestPlugin"));
  }

  [Test]
  public void TestDownloadPlugin_NotDownloadedLocalOnly() {
    Assert.ThrowsAsync<PluginNotFoundException>(() => _pluginManagementService.DownloadPlugin("TestPlugin",
                                                  new SemVersion(1, 0, 0), null, "5.5", ["Win54"]));
  }

  [Test]
  public void TestDownloadPlugin_NothingOnCloud() {
    _pluginsApi.Setup(x => x.GetLatestVersionsAsync("TestPlugin", "1.0.0", 1, 100, CancellationToken.None))
        .ReturnsAsync(new Page<PluginVersionInfo>([]));
    Assert.ThrowsAsync<PluginNotFoundException>(() => _pluginManagementService.DownloadPlugin("TestPlugin",
                                                  new SemVersion(1, 0, 0), 0, "5.5", ["Win54"]));
  }

  [Test]
  public async Task TestDownloadPlugin() {
    var pluginDetails = new PluginVersionDetails {
        PluginId = Guid.NewGuid(),
        Name = "TestPlugin",
        VersionId = Guid.NewGuid(),
        Version = new SemVersion(1, 0, 0),
        Dependencies = []
    };

    _engineService.Setup(x => x.GetInstalledEngine("5.5"))
        .Returns(new InstalledEngine("5.5", new Version(5, 5), null!));
    _pluginsApi.Setup(x => x.GetLatestVersionsAsync("TestPlugin", "1.0.0", 1, 100, CancellationToken.None))
        .ReturnsAsync(new Page<PluginVersionInfo>([pluginDetails]));
    _pluginsApi.Setup(x => x.GetPluginPatchesAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync([]);

    _sourceDownloadService
        .Setup(x => x.DownloadAndExtractSources(It.IsAny<SourceLocation>(), It.IsAny<IDirectoryInfo>()))
        .Returns((SourceLocation _, IDirectoryInfo directory) => {
          directory.File("TestPlugin.uplugin").Create();
          return Task.CompletedTask;
        });

    _binaryCacheService.Setup(x => x.CacheBuiltPlugin(It.IsAny<PluginManifest>(), It.IsAny<IDirectoryInfo>(),
                                                      It.IsAny<IReadOnlyList<string>>(), It.IsAny<string>(),
                                                      It.IsAny<IReadOnlyCollection<string>>()))
        .ReturnsAsync(new PluginBuildInfo {
            PluginName = pluginDetails.Name,
            PluginVersion = pluginDetails.Version,
            BuiltOn = DateTimeOffset.Now,
            DirectoryName = "",
            EngineVersion = "5.5"
        });

    var result = await _pluginManagementService.DownloadPlugin("TestPlugin",
                                                               new SemVersion(1, 0, 0), 0, "5.5", ["Win64"]);
    Assert.That(result.PluginName, Is.EqualTo(pluginDetails.Name));
  }
}