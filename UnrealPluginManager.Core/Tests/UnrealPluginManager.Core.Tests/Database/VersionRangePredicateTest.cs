using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using Microsoft.Extensions.DependencyInjection;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Tests.Database;

public class VersionRangePredicateTest {
  private UnrealPluginManagerContext _context;
  private ServiceProvider _serviceProvider;

  [SetUp]
  public void Setup() {
    var services = new ServiceCollection();

    var mockFilesystem = new MockFileSystem();
    services.AddSingleton<IFileSystem>(mockFilesystem);

    _context = new TestUnrealPluginManagerContext();
    _context.Database.EnsureCreated();
    services.AddSingleton(_context);
    _serviceProvider = services.BuildServiceProvider();
  }

  [TearDown]
  public void TearDown() {
    _serviceProvider.Dispose();
    _context.Dispose();
  }

  private static void AddTestPlugins(UnrealPluginManagerContext context) {
    var versions = new List<SemVersion> {
        SemVersion.Parse("1.0.0"),
        SemVersion.Parse("1.2.2"),
        SemVersion.Parse("1.2.3"),
        SemVersion.Parse("1.12.0"),
        SemVersion.Parse("1.12.0-rc.1"),
        SemVersion.Parse("1.12.0-rc.2"),
        SemVersion.Parse("1.12.0-rc.3"),
        SemVersion.Parse("1.12.0-rc.14"),
        SemVersion.Parse("2.0.0"),
        SemVersion.Parse("2.9.12"),
        SemVersion.Parse("3.0.0"),
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
  }

  [Test]
  public void AllDoesntFilterOutPrereleaseVersions() {
    var context = _serviceProvider.GetRequiredService<UnrealPluginManagerContext>();
    AddTestPlugins(context);

    var validVersions = context.PluginVersions
        .WhereVersionInRange(SemVersionRange.All)
        .Select(x => x.Version)
        .ToList();
    Assert.That(validVersions, Has.Count.EqualTo(11));
  }

  [Test]
  public void AllReleaseFiltersOutPrereleaseVersions() {
    var context = _serviceProvider.GetRequiredService<UnrealPluginManagerContext>();
    AddTestPlugins(context);

    var validVersions = context.PluginVersions
        .WhereVersionInRange(SemVersionRange.AllRelease)
        .Select(x => x.Version)
        .ToList();
    Assert.That(validVersions, Has.Count.EqualTo(7));
    Assert.That(validVersions, Has.All.Matches<SemVersion>(x => !x.IsPrerelease));
  }

  [Test]
  public void InclusiveRangeIncludesEndpoints() {
    var context = _serviceProvider.GetRequiredService<UnrealPluginManagerContext>();
    AddTestPlugins(context);

    var validVersions = context.PluginVersions
        .WhereVersionInRange(SemVersionRange.Parse(">=1.0.0 <=2.9.12"))
        .Select(x => x.Version)
        .ToList();
    Assert.That(validVersions, Has.Count.EqualTo(5));
    Assert.That(validVersions, Has.All.Matches<SemVersion>(x => !x.IsPrerelease));
    Assert.That(validVersions, Has.Member(SemVersion.Parse("1.0.0")));
    Assert.That(validVersions, Has.Member(SemVersion.Parse("2.9.12")));
  }

  [Test]
  public void InclusiveRangeExcludesEndpoints() {
    var context = _serviceProvider.GetRequiredService<UnrealPluginManagerContext>();
    AddTestPlugins(context);

    var validVersions = context.PluginVersions
        .WhereVersionInRange(SemVersionRange.Parse(">1.0.0 <3.0.0"))
        .Select(x => x.Version)
        .ToList();
    Assert.That(validVersions, Has.Count.EqualTo(5));
    Assert.That(validVersions, Has.All.Matches<SemVersion>(x => !x.IsPrerelease));
    Assert.That(validVersions, Has.None.Matches<SemVersion>(x => x == SemVersion.Parse("1.0.0")));
    Assert.That(validVersions, Has.None.Matches<SemVersion>(x => x == SemVersion.Parse("3.0.0")));
  }

  [Test]
  public void PrereleaseElementsAreIncludedCorrectly() {
    var context = _serviceProvider.GetRequiredService<UnrealPluginManagerContext>();
    AddTestPlugins(context);

    var validVersions = context.PluginVersions
        .WhereVersionInRange(SemVersionRange.Parse(">=1.12.0-rc.1 <=1.12.0-rc.14"))
        .Select(x => x.Version)
        .ToList();
    Assert.That(validVersions, Has.Count.EqualTo(4));
    Assert.That(validVersions, Has.Exactly(1).Matches<SemVersion>(x => x == SemVersion.Parse("1.12.0-rc.1")));
    Assert.That(validVersions, Has.Exactly(1).Matches<SemVersion>(x => x == SemVersion.Parse("1.12.0-rc.14")));
  }

  [Test]
  public void PrereleaseElementsAreExcludedCorrectly() {
    var context = _serviceProvider.GetRequiredService<UnrealPluginManagerContext>();
    AddTestPlugins(context);

    var validVersions = context.PluginVersions
        .WhereVersionInRange(SemVersionRange.Parse(">1.12.0-rc.1 <1.12.0-rc.14"))
        .Select(x => x.Version)
        .ToList();
    Assert.That(validVersions, Has.Count.EqualTo(2));
    Assert.That(validVersions, Has.None.Matches<SemVersion>(x => x == SemVersion.Parse("1.12.0-rc.1")));
    Assert.That(validVersions, Has.None.Matches<SemVersion>(x => x == SemVersion.Parse("1.12.0-rc.14")));
  }
}