using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Core.Tests.Database;

public class PluginVersionSortOrderTest {
    
    private ServiceProvider _serviceProvider;

    [SetUp]
    public void Setup() {
        var services = new ServiceCollection();

        var mockFilesystem = new MockFileSystem();
        services.AddSingleton<IFileSystem>(mockFilesystem);

        services.AddDbContext<UnrealPluginManagerContext, TestUnrealPluginManagerContext>();
        _serviceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public void TearDown() {
        _serviceProvider.Dispose();
    }
    
    [Test]
    public void TestPluginVersionsOrderCorrectly() {
        var context = _serviceProvider.GetRequiredService<UnrealPluginManagerContext>();
        
        var versions = new List<SemVersion> {
            SemVersion.Parse("1.0.0"),
            SemVersion.Parse("1.2.2"),
            SemVersion.Parse("1.2.3"),
            SemVersion.Parse("1.12.0"),
            SemVersion.Parse("1.12.0-rc.1"),
            SemVersion.Parse("1.12.0-rc.3"),
            SemVersion.Parse("1.12.0-rc.14")
        };
        var plugin = new Plugin {
            Name = "Test Plugin",
            Versions = versions
                .Select(x => new PluginVersion {
                    Version = x
                })
                .ToList()
        };

        context.Plugins.Add(plugin);
        context.SaveChanges();

        var query = context.PluginVersions
            .OrderByVersion()
            .Select(x => x.Version);
        var retrievedPlugins = query.ToList();
        
        
        Assert.That(retrievedPlugins, Is.EqualTo(versions
            .OrderBy(x => x, SemVersion.PrecedenceComparer)
            .ToList()));
    }

    [Test]
    public void TestPluginVersionsOrderCorrectlyDescending() {
        var context = _serviceProvider.GetRequiredService<UnrealPluginManagerContext>();
        
        var versions = new List<SemVersion> {
            SemVersion.Parse("1.0.0"),
            SemVersion.Parse("1.2.2"),
            SemVersion.Parse("1.2.3"),
            SemVersion.Parse("1.12.0"),
            SemVersion.Parse("1.12.0-rc.1"),
            SemVersion.Parse("1.12.0-rc.3"),
            SemVersion.Parse("1.12.0-rc.14")
        };
        var plugin = new Plugin {
            Name = "Test Plugin",
            Versions = versions
                .Select(x => new PluginVersion {
                    Version = x
                })
                .ToList()
        };

        context.Plugins.Add(plugin);
        context.SaveChanges();

        var query = context.PluginVersions
            .OrderByVersionDecending()
            .Select(x => x.Version);
        var retrievedPlugins = query.ToList();
        
        
        Assert.That(retrievedPlugins, Is.EqualTo(versions
            .OrderByDescending(x => x, SemVersion.PrecedenceComparer)
            .ToList()));
    }
    
}