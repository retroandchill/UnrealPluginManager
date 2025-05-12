using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using LanguageExt;
using LanguageExt.UnsafeValueAccess;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Semver;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Local.Database;
using UnrealPluginManager.Local.Services;
using UnrealPluginManager.Local.Tests.Utils;

namespace UnrealPluginManager.Local.Tests.Services;

public class BinaryCacheServiceTests {
  private ServiceProvider _serviceProvider;
  private MockFileSystem _fileSystem;
  private Mock<IPluginService> _pluginService;
  private Mock<IEngineService> _engineService;
  private Mock<IStorageService> _storageService;
  private LocalUnrealPluginManagerContext _dbContext;
  private IBinaryCacheService _service;

  [SetUp]
  public void Setup() {
    var services = new ServiceCollection();

    _fileSystem = new MockFileSystem();
    services.AddSingleton<IFileSystem>(_fileSystem);

    _pluginService = new Mock<IPluginService>();
    services.AddSingleton(_pluginService.Object);

    _engineService = new Mock<IEngineService>();
    services.AddSingleton(_engineService.Object);

    _storageService = new Mock<IStorageService>();
    services.AddSingleton(_storageService.Object);

    _dbContext = new TestLocalUnrealPluginManagerContext(_storageService.Object);
    _dbContext.Database.EnsureCreated();
    services.AddSingleton(_dbContext);

    services.AddSingleton<IBinaryCacheService, BinaryCacheService>();

    _serviceProvider = services.BuildServiceProvider();
    _service = _serviceProvider.GetRequiredService<IBinaryCacheService>();
  }

  [TearDown]
  public void TearDown() {
    _dbContext.Dispose();
    _serviceProvider.Dispose();
  }

  [Test]
  public async Task CacheBuiltPlugin_WithValidManifest_CreatesNewBuildRecord() {
    // Arrange
    var manifest = new PluginManifest {
        Name = "TestPlugin",
        Version = SemVersion.Parse("1.0.0"),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/EpicGames/UnrealEngine/tree/5.1.0/Engine/Plugins/Marketplace/TestPlugin"),
            Sha = "FakeSha"
        },
        Dependencies = []
    };

    var directory = _fileSystem.DirectoryInfo.New("/test/plugin");
    var patches = new List<string> {
        "patch1.diff"
    };
    const string engineVersion = "5.1.0";
    var platforms = new List<string> {
        "Win64"
    };

    var plugin = new Plugin {
        Id = Guid.CreateVersion7(),
        Name = "TestPlugin"
    };
    var pluginVersion = new PluginVersion {
        Id = Guid.CreateVersion7(),
        Parent = plugin,
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/EpicGames/UnrealEngine/tree/5.1.0/Engine/Plugins/Marketplace/TestPlugin"),
            Sha = "FakeSha"
        },
        Dependencies = []
    };

    _pluginService.Setup(x => x.GetPluginVersionInfo(manifest.Name, manifest.Version))
        .ReturnsAsync(Option<PluginVersionInfo>.None);

    await _dbContext.Plugins.AddAsync(plugin);
    await _dbContext.PluginVersions.AddAsync(pluginVersion);
    await _dbContext.SaveChangesAsync();

    // Act
    var result = await _service.CacheBuiltPlugin(manifest, directory, patches, engineVersion, platforms);

    Assert.Multiple(() => {
      // Assert
      Assert.That(result, Is.Not.Null);
      Assert.That(result.PluginName, Is.EqualTo(manifest.Name));
      Assert.That(result.PluginVersion, Is.EqualTo(manifest.Version));
      Assert.That(result.EngineVersion, Is.EqualTo(engineVersion));
      Assert.That(result.DirectoryName, Is.EqualTo(directory.FullName));
      Assert.That(result.Platforms, Is.EqualTo(platforms));
    });

    var savedBuild = await _service.GetCachedPluginBuild(plugin.Name, pluginVersion.Version, engineVersion, platforms)
        .Map(x => x.ValueUnsafe());
    Assert.That(savedBuild, Is.Not.Null);
    Assert.That(savedBuild.Platforms, Has.Count.EqualTo(platforms.Count));
  }
}