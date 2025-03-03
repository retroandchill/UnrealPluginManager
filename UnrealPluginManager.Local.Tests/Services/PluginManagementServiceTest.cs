using System.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UnrealPluginManager.Core.Model.Plugins;
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

    _pluginsApi = new Mock<IPluginsApi>();
    services.AddSingleton(_pluginsApi.Object);
    services.AddSingleton<IApiAccessor>(_pluginsApi.Object);

    var console = new Mock<IConsole>();
    services.AddSingleton(console.Object);

    services.AddSingleton(_pluginsApi.Object);
    services.AddScoped<IRemoteService, RemoteService>();
    services.AddScoped<IPluginManagementService, PluginManagementService>();

    _serviceProvider = services.BuildServiceProvider();
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
}