using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Tests.Helpers;

namespace UnrealPluginManager.Core.Tests.Services;

public class PluginServiceTests {
    
    private ServiceProvider _serviceProvider;
    
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
    }

    [TearDown]
    public void TearDown() {
        _serviceProvider.Dispose();
    }

    [Test]
    public async Task TestGetPlugins() {
        var context = _serviceProvider.GetRequiredService<UnrealPluginManagerContext>();
        var plugins = Enumerable.Range(1, 10)
            .SelectMany(i => new [] { 
                new Plugin {
                    Name = "Plugin" + i,
                    FriendlyName = "Plugin" + i,
                    Version = new SemVersion(1, 0, 0)
                },
                new Plugin {
                    Name = "Plugin" + i,
                    FriendlyName = "Plugin" + i,
                    Version = new SemVersion(1, 2, 2)
                } 
            });
        context.AddRange(plugins);
        await context.SaveChangesAsync();
        
        var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
        var summaries = await pluginService.GetPluginSummaries();
        Assert.That(summaries, Has.Count.EqualTo(10));
    }
    
    [Test]
    public async Task TestAddPlugins() {
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

        var plugin1List = await pluginService.GetDependencyList("Plugin1");
        Assert.That(plugin1List, Has.Count.EqualTo(1));
        Assert.That(plugin1List[0].Name, Is.EqualTo("Plugin1"));
        
        var plugin2List = await pluginService.GetDependencyList("Plugin2");
        Assert.That(plugin2List, Has.Count.EqualTo(2));
        var plugin2Names = plugin2List.Select(x => x.Name).ToList();
        Assert.That(plugin2Names, Does.Contain("Plugin1"));
        Assert.That(plugin2Names, Does.Contain("Plugin2"));
        
        var plugin3List = await pluginService.GetDependencyList("Plugin3");
        Assert.That(plugin3List, Has.Count.EqualTo(3));
        var plugin3Names = plugin3List.Select(x => x.Name).ToList();
        Assert.That(plugin3Names, Does.Contain("Plugin1"));
        Assert.That(plugin3Names, Does.Contain("Plugin2"));
        Assert.That(plugin3Names, Does.Contain("Plugin3"));
    }
    
    [Test]
    public async Task TestAddPluginVersions() {
        var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
        await pluginService.SetupVersionResolutionTree();
        
        var dependencyGraph = await pluginService.GetDependencyList("App");
        Assert.That(dependencyGraph, Has.Count.EqualTo(5));
        Assert.Multiple(() => {
            Assert.That(dependencyGraph.Find(x => x.Name == "Threads")?.Version, Is.EqualTo(new SemVersion(2, 0, 0)));
            Assert.That(dependencyGraph.Find(x => x.Name == "StdLib")?.Version, Is.EqualTo(new SemVersion(4, 0, 0)));
            Assert.That(dependencyGraph.Find(x => x.Name == "Sql")?.Version, Is.EqualTo(new SemVersion(2, 0, 0)));
            Assert.That(dependencyGraph.Find(x => x.Name == "Http")?.Version, Is.EqualTo(new SemVersion(4, 0, 0)));
            Assert.That(dependencyGraph.Find(x => x.Name == "App")?.Version, Is.EqualTo(new SemVersion(1, 0, 0)));
        });
    }
}