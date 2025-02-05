using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Controllers;

namespace UnrealPluginManager.Server.Tests.Controllers;

public class TestPluginController {
    
    private ServiceProvider _serviceProvider;
    private PluginsController _pluginsController;
    
    [SetUp]
    public void Setup() {
        var services = new ServiceCollection();
        services.AddDbContext<UnrealPluginManagerContext>(options => options
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors()
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .LogTo(Console.WriteLine));
        services.AddScoped<IPluginService, PluginService>();
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
        });
        
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
        });
        
        await pluginService.AddPlugin("Plugin3", new PluginDescriptor {
            Version = 1,
            VersionName = new SemVersion(1, 0, 0),
            Plugins = [
                new PluginReferenceDescriptor {
                    Name = "Plugin2",
                    PluginType = PluginType.Provided
                }
            ]
        });
        
        await pluginService.AddPlugin("Plugin3", new PluginDescriptor {
            Version = 1,
            VersionName = new SemVersion(1, 2, 1),
            Plugins = [
                new PluginReferenceDescriptor {
                    Name = "Plugin2",
                    PluginType = PluginType.Provided
                }
            ]
        });
        
        await pluginService.AddPlugin("Plugin4", new PluginDescriptor {
            Version = 1,
            VersionName = new SemVersion(1, 2, 1),
            Plugins = []
        });

        var plugin1List = await _pluginsController.GetDependencyTree("Plugin1");
        Assert.That(plugin1List, Has.Count.EqualTo(1));
        Assert.That(plugin1List[0].Name, Is.EqualTo("Plugin1"));
        
        var plugin2List = await _pluginsController.GetDependencyTree("Plugin2");
        Assert.That(plugin2List, Has.Count.EqualTo(2));
        var plugin2Names = plugin2List.Select(x => x.Name).ToList();
        Assert.That(plugin2Names, Does.Contain("Plugin1"));
        Assert.That(plugin2Names, Does.Contain("Plugin2"));
        
        var plugin3List = await _pluginsController.GetDependencyTree("Plugin3");
        Assert.That(plugin3List, Has.Count.EqualTo(3));
        var plugin3Names = plugin3List.Select(x => x.Name).ToList();
        Assert.That(plugin3Names, Does.Contain("Plugin1"));
        Assert.That(plugin3Names, Does.Contain("Plugin2"));
        Assert.That(plugin3Names, Does.Contain("Plugin3"));

        var allPluginsList = await _pluginsController.Get();
        Assert.That(allPluginsList, Has.Count.EqualTo(4));
    }
    
}