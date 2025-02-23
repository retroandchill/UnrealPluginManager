using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UnrealPluginManager.Cli.Factories;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Pagination;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.WebClient.Api;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Cli.Tests.Services;

public class RemoteCallServiceTest {
    
    private ServiceProvider _serviceProvider;
    private OrderedDictionary<string, Mock<IPluginsApi>> _remotes;
    private Mock<IApiAccessorFactory<IPluginsApi>> _factory;
    private IRemoteCallService _remoteCallService;

    [SetUp]
    public void Setup() {
        var services = new ServiceCollection();

        _remotes = new OrderedDictionary<string, Mock<IPluginsApi>> {
            {
                "default", new Mock<IPluginsApi>()
            }, {
                "alt", new Mock<IPluginsApi>()
            }, {
                "unaccessible", new Mock<IPluginsApi>()
            }
        };

        _factory = new Mock<IApiAccessorFactory<IPluginsApi>>();

        var mockedRemotes = _remotes.ToOrderedDictionary(x => x.Object);
        _factory.Setup(x => x.GetAccessors()).Returns(Task.FromResult(mockedRemotes));
        
        services.AddSingleton(_factory.Object);
        services.AddScoped<IRemoteCallService, RemoteCallService>();
        
        _serviceProvider = services.BuildServiceProvider();
        _remoteCallService = _serviceProvider.GetRequiredService<IRemoteCallService>();
        
        AddPluginsToRemote("default", 300);
        AddPluginsToRemote("alt", 50);
        _remotes["unaccessible"].Setup(x => x.GetPluginsAsync(It.IsAny<string>(), It.IsAny<int?>(),
                It.Is(100, EqualityComparer<int>.Default), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ApiException(404, "Unreachable"));
    }

    [TearDown]
    public void TearDown() {
        _serviceProvider.Dispose();
    }

    private void AddPluginsToRemote(string remote, int totalCount) {
        var plugins = Enumerable.Range(0, totalCount)
            .Select(i => new PluginOverview {
                Id = (ulong)i + 1,
                Name = $"Plugin{i + 1}",
                FriendlyName = $"Plugin {i + 1}",
                Versions = []
            })
            .AsPages(100)
            .ToList();
        _remotes[remote].Setup(x => x.GetPluginsAsync(It.IsAny<string>(), It.IsAny<int?>(),
                It.Is(100, EqualityComparer<int>.Default), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .Returns((string _, int x, int _, int _, CancellationToken _) => Task.FromResult(plugins[x - 1]));
    }
    
    [Test]
    public async Task TestGetPluginsFromAllRemotes() {
        var plugins = await _remoteCallService.GetPlugins("*");
        Assert.That(plugins, Has.Count.EqualTo(3));
        Assert.That(plugins, Does.ContainKey("default"));
        Assert.That(plugins["default"].IsSucc, Is.True);
        Assert.That(plugins["default"].Get(), Has.Count.EqualTo(300));
        
        Assert.That(plugins, Does.ContainKey("alt"));
        Assert.That(plugins["alt"].IsSucc, Is.True);
        Assert.That(plugins["alt"].Get(), Has.Count.EqualTo(50));
        
        Assert.That(plugins, Does.ContainKey("unaccessible"));
        Assert.That(plugins["unaccessible"].IsFail, Is.True);
    }

    [Test]
    public async Task TestGetPluginsFromSingleRemote() {
        var plugins = await _remoteCallService.GetPlugins("default", "*");
        Assert.That(plugins, Has.Count.EqualTo(300));
        
        plugins = await _remoteCallService.GetPlugins("alt", "*");
        Assert.That(plugins, Has.Count.EqualTo(50));
        
        Assert.ThrowsAsync<ArgumentException>(() => _remoteCallService.GetPlugins("invalid", "*"));
    }
    
}