using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Core.Tests.Services;

public class PluginServiceTests {
    
    private ServiceProvider _serviceProvider;
    
    [SetUp]
    public void Setup() {
        var services = new ServiceCollection();
        services.AddDbContext<UnrealPluginManagerContext>(options => options.UseInMemoryDatabase("TestDb"));
        services.AddScoped<IPluginService, PluginService>();
        _serviceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public void TearDown() {
        _serviceProvider.Dispose();
    }

    [Test]
    public void TestGetPlugins() {
        var context = _serviceProvider.GetRequiredService<UnrealPluginManagerContext>();
        var plugins = Enumerable.Range(1, 10)
            .Select(i => new Plugin {
                Name = "Plugin" + i,
                FriendlyName = "Plugin" + i,
                Version = new Version(1, 0, 0)
            });
        context.AddRange(plugins);
        context.SaveChanges();
        
        var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
        var summaries = pluginService.GetPluginSummaries();
        Assert.AreEqual(10, summaries.Count());
    }
    
    [Test]
    public void TestAddPlugins() {
        var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
        pluginService.AddPlugin("Plugin1", new PluginDescriptor {
            Version = 1,
            VersionName = new Version(1, 0, 0)
        });
        
        pluginService.AddPlugin("Plugin2", new PluginDescriptor {
            Version = 1,
            VersionName = new Version(1, 0, 0),
            Plugins = [
                new PluginReferenceDescriptor {
                    Name = "Plugin1",
                    PluginType = PluginType.Provided,
                    VersionMatcher = ">=1.0.0"
                },
                new PluginReferenceDescriptor {
                    Name = "Paper2D",
                    PluginType = PluginType.Engine,
                    VersionMatcher = ">=1.0.0"
                }
            ]
        });
        
        pluginService.AddPlugin("Plugin3", new PluginDescriptor {
            Version = 1,
            VersionName = new Version(1, 0, 0),
            Plugins = [
                new PluginReferenceDescriptor {
                    Name = "Plugin2",
                    PluginType = PluginType.Provided
                }
            ]
        });

        var plugin1List = pluginService.GetDependencyList("Plugin1").ToList();
        Assert.AreEqual(1, plugin1List.Count);
        Assert.AreEqual("Plugin1", plugin1List[0].Name);
        
        var plugin2List = pluginService.GetDependencyList("Plugin2").ToList();
        Assert.AreEqual(2, plugin2List.Count);
        Assert.Contains("Plugin1", plugin2List.Select(x => x.Name).ToList());
        Assert.Contains("Plugin2", plugin2List.Select(x => x.Name).ToList());
        
        var plugin3List = pluginService.GetDependencyList("Plugin3").ToList();
        Assert.AreEqual(3, plugin3List.Count);
        Assert.Contains("Plugin1", plugin3List.Select(x => x.Name).ToList());
        Assert.Contains("Plugin2", plugin3List.Select(x => x.Name).ToList());
        Assert.Contains("Plugin3", plugin3List.Select(x => x.Name).ToList());
    }
    
    [Test]
    public void TestAddPluginVersions() {
        var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
        pluginService.AddPlugin("Plugin1", new PluginDescriptor {
            Version = 1,
            VersionName = new Version(1, 0, 0)
        });
        pluginService.AddPlugin("Plugin1", new PluginDescriptor {
            Version = 1,
            VersionName = new Version(1, 10, 3)
        });
        pluginService.AddPlugin("Plugin1", new PluginDescriptor {
            Version = 1,
            VersionName = new Version(2, 0, 1)
        });
        
        pluginService.AddPlugin("Plugin2", new PluginDescriptor {
            Version = 1,
            VersionName = new Version(1, 0, 0),
            Plugins = [
                new PluginReferenceDescriptor {
                    Name = "Plugin1",
                    PluginType = PluginType.Provided,
                    VersionMatcher = ">=1.0.0"
                },
                new PluginReferenceDescriptor {
                    Name = "Paper2D",
                    PluginType = PluginType.Engine,
                    VersionMatcher = ">=1.0.0"
                }
            ]
        });
        
        pluginService.AddPlugin("Plugin3", new PluginDescriptor {
            Version = 1,
            VersionName = new Version(1, 0, 0),
            Plugins = [
                new PluginReferenceDescriptor {
                    Name = "Plugin2",
                    PluginType = PluginType.Provided
                }
            ]
        });

        var plugin1List = pluginService.GetDependencyList("Plugin1").ToList();
        Assert.AreEqual(1, plugin1List.Count);
        Assert.AreEqual("Plugin1", plugin1List[0].Name);
        
        var plugin2List = pluginService.GetDependencyList("Plugin2").ToList();
        Assert.AreEqual(2, plugin2List.Count);
        Assert.Contains("Plugin1", plugin2List.Select(x => x.Name).ToList());
        Assert.Contains("Plugin2", plugin2List.Select(x => x.Name).ToList());
        
        var plugin3List = pluginService.GetDependencyList("Plugin3").ToList();
        Assert.AreEqual(3, plugin3List.Count);
        Assert.Contains("Plugin1", plugin3List.Select(x => x.Name).ToList());
        Assert.Contains("Plugin2", plugin3List.Select(x => x.Name).ToList());
        Assert.Contains("Plugin3", plugin3List.Select(x => x.Name).ToList());
    }
}