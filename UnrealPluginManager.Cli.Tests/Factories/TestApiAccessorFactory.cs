using Microsoft.Extensions.DependencyInjection;
using Moq;
using UnrealPluginManager.Cli.Config;
using UnrealPluginManager.Cli.Factories;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Cli.Utils;
using UnrealPluginManager.WebClient.Api;

namespace UnrealPluginManager.Cli.Tests.Factories;

public class TestApiAccessorFactory {
    
    private ServiceProvider _serviceProvider;
    private Mock<IRemoteService> _remoteService;

    [SetUp]
    public void Setup() {
        var services = new ServiceCollection();
        
        _remoteService = new Mock<IRemoteService>();
        _remoteService.Setup(x => x.GetAllRemotes())
            .Returns(Task.FromResult(new OrderedDictionary<string, RemoteConfig> {
                {
                    "default",
                    new Uri("https://unrealpluginmanager.com")
                }, {
                    "local",
                    new Uri("https://localhost:8080")
                }
            }));
        
        services.AddSingleton(_remoteService.Object);
        services.AddApiFactories();
        
        _serviceProvider = services.BuildServiceProvider();
    }
    
    [TearDown]
    public void TearDown() {
        _serviceProvider.Dispose();
    }

    [Test]
    public async Task TestCreateApiAccessors() {
        var pluginsAccessorFactory = _serviceProvider.GetRequiredService<IApiAccessorFactory<IPluginsApi>>();
        var plugins = await pluginsAccessorFactory.GetAccessors();
        Assert.That(plugins, Has.Count.EqualTo(2));
        
        var storageAccessorFactory = _serviceProvider.GetRequiredService<IApiAccessorFactory<IStorageApi>>();
        var storage = await storageAccessorFactory.GetAccessors();
        Assert.That(storage, Has.Count.EqualTo(2));
    }
    
    
}