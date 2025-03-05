using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Semver;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Resolution;
using UnrealPluginManager.Core.Pagination;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Local.Config;
using UnrealPluginManager.Local.Services;
using UnrealPluginManager.Local.Tests.Mocks;
using UnrealPluginManager.WebClient.Api;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Local.Tests.Services;

public class PluginManagementServiceTest {
  private ServiceProvider _serviceProvider;
  private Mock<IStorageService> _storageService;
  private Mock<IPluginsApi> _pluginsApi;
  private Mock<IPluginService> _pluginService;
  private Mock<IEngineService> _engineService;
  private IPluginManagementService _pluginManagementService;

  [SetUp]
  public void Setup() {
    var services = new ServiceCollection();

    _storageService = new Mock<IStorageService>();
    services.AddSingleton(_storageService.Object);
    _storageService.Setup(x => x.GetConfig(It.IsAny<string>(), It.IsAny<OrderedDictionary<string, RemoteConfig>>()))
        .Returns(new OrderedDictionary<string, RemoteConfig> {
            ["default"] = new() {
                Url = new Uri("https://unrealpluginmanager.com")
            },
            ["alt"] = new() {
                Url = new Uri("https://github.com/api/v1/repos/EpicGames/UnrealEngine/releases/latest")
            },
            ["unaccessible"] = new() {
                Url = new Uri("https://unrealpluginmanager.com/invalid")
            }
        });

    services.AddSingleton<IApiTypeResolver, MockTypeResolver>();

    _pluginService = new Mock<IPluginService>();
    services.AddSingleton(_pluginService.Object);

    _engineService = new Mock<IEngineService>();
    services.AddSingleton(_engineService.Object);

    _pluginsApi = new Mock<IPluginsApi>();
    services.AddSingleton(_pluginsApi.Object);
    services.AddSingleton<IApiAccessor>(_pluginsApi.Object);

    var console = new Mock<IConsole>();
    services.AddSingleton(console.Object);

    services.AddSingleton(_pluginsApi.Object);
    services.AddScoped<IRemoteService, RemoteService>();
    services.AddScoped<IPluginManagementService, PluginManagementService>();

    _serviceProvider = services.BuildServiceProvider();
    
    var realPluginService = new PluginService(null, null, null, null);
    _pluginService.Setup(x => x.GetDependencyList(It.IsAny<IDependencyChainNode>(), It.IsAny<DependencyManifest>()))
        .Returns((IDependencyChainNode root, DependencyManifest manifest) => realPluginService.GetDependencyList(root, manifest));
    
    _pluginManagementService = _serviceProvider.GetRequiredService<IPluginManagementService>();

    var pageList = AddPluginsToRemote(300)
        .Concat(AddPluginsToRemote(50))
        .ToList();

    var pageIndex = 0;
    _pluginsApi.Setup(x => x.GetPluginsAsync(It.IsAny<string>(), It.IsAny<int?>(),
                                             It.Is(100, EqualityComparer<int>.Default), It.IsAny<int>(),
                                             It.IsAny<CancellationToken>()))
        .Returns((string? _, int? _, int? _, int _, CancellationToken _) => {
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
            Id = (ulong)i + 1,
            Name = $"Plugin{i + 1}",
            FriendlyName = $"Plugin {i + 1}",
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
                Type = PluginType.Provided,
                PluginVersion = SemVersionRange.Parse("=2.0.0")
            },
            new PluginDependency {
                PluginName = "Threads",
                Type = PluginType.Provided,
                PluginVersion = SemVersionRange.Parse("=2.0.0")
            },
            new PluginDependency {
                PluginName = "Http",
                Type = PluginType.Provided,
                PluginVersion = SemVersionRange.Parse(">=3.0.0 <=4.0.0")
            },
            new PluginDependency {
                PluginName = "StdLib",
                Type = PluginType.Provided,
                PluginVersion = SemVersionRange.Parse("=4.0.0")
            }
        ]
    };

    _engineService.Setup(x => x.GetInstalledPlugins(null))
        .Returns(new List<PluginIdentifier> {
            new("StdLib", new SemVersion(4, 0, 0)),
            new("Http", new SemVersion(3, 0, 0)),
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
                                                  VersionId = 1,
                                                  PluginId = 1,
                                                  Name = x.Key,
                                                  Version = y,
                                                  Dependencies = []
                                              })
                                  .ToList())
        }));

    _pluginsApi.Setup(x => x.GetCandidateDependenciesAsync(
                          It.IsAny<List<PluginDependency>>(),
                          It.IsAny<int>(), It.IsAny<CancellationToken>()))
        .Returns(Task.FromResult(new DependencyManifest {
            FoundDependencies = allPlugins.Where(x => x.Key is not "Http" and not "StdLib")
                .ToDictionary(x => x.Key, x => x.Value
                                  .Select(y =>
                                              new PluginVersionInfo {
                                                  VersionId = 1,
                                                  PluginId = 1,
                                                  Name = x.Key,
                                                  Version = y,
                                                  Dependencies = []
                                              })
                                  .ToList())
        }));

    var result = await _pluginManagementService.GetPluginsToInstall(root, null);
    Assert.That(result, Is.InstanceOf<ResolvedDependencies>());
    var dependencyGraph = ((ResolvedDependencies)result).SelectedPlugins;
    Assert.That(dependencyGraph, Has.Count.EqualTo(4));
    Assert.Multiple(() => {
      Assert.That(dependencyGraph.Find(x => x.Name == "Threads")?.Version, Is.EqualTo(new SemVersion(2, 0, 0)));
      Assert.That(dependencyGraph.Find(x => x.Name == "StdLib")?.Version, Is.EqualTo(new SemVersion(4, 0, 0)));
      Assert.That(dependencyGraph.Find(x => x.Name == "Sql")?.Version, Is.EqualTo(new SemVersion(2, 0, 0)));
      Assert.That(dependencyGraph.Find(x => x.Name == "Http")?.Version, Is.EqualTo(new SemVersion(3, 0, 0)));
    });
  }
}