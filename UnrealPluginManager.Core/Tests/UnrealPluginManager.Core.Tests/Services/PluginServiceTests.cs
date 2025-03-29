using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Compression;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Retro.SimplePage;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Database.Entities.Storage;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Files;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Resolution;
using UnrealPluginManager.Core.Model.Storage;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Tests.Database;
using UnrealPluginManager.Core.Tests.Helpers;
using UnrealPluginManager.Core.Tests.Mocks;

namespace UnrealPluginManager.Core.Tests.Services;

public class PluginServiceTests {
  private static readonly JsonSerializerOptions JsonOptions = new() {
      AllowTrailingCommas = true
  };

  private MockFileSystem _mockFilesystem;
  private UnrealPluginManagerContext _context;
  private ServiceProvider _serviceProvider;
  private IStorageService _storageService;

  [SetUp]
  public void Setup() {
    var services = new ServiceCollection();

    _mockFilesystem = new MockFileSystem();
    services.AddSingleton<IFileSystem>(_mockFilesystem);

    services.AddSingleton<IJsonService>(new JsonService(JsonOptions));

    _context = new TestUnrealPluginManagerContext();
    _context.Database.EnsureCreated();
    services.AddSingleton(_context);
    services.AddScoped<IPluginService, PluginService>();
    services.AddScoped<IPluginStructureService, PluginStructureService>();
    services.AddSingleton<IStorageService, MockStorageService>();
    _serviceProvider = services.BuildServiceProvider();

    _storageService = _serviceProvider.GetRequiredService<IStorageService>();
  }

  [TearDown]
  public void TearDown() {
    _serviceProvider.Dispose();
    _context.Dispose();
  }

  [Test]
  public async Task TestGetPlugins() {
    var context = _serviceProvider.GetRequiredService<UnrealPluginManagerContext>();
    var plugins = Enumerable.Range(1, 10)
        .SelectMany(i => new[] {
            new Plugin {
                Name = "Plugin" + i,
                FriendlyName = "Plugin" + i,
                Versions = [
                    new PluginVersion {
                        Version = new SemVersion(1, 0, 0),
                        Source = new FileResource {
                            OriginalFilename = "Source.zip",
                            StoredFilename = "Dummy"
                        }
                    },
                    new PluginVersion {
                        Version = new SemVersion(1, 2, 2),
                        Source = new FileResource {
                            OriginalFilename = "Source.zip",
                            StoredFilename = "Dummy"
                        }
                    }
                ]
            }
        });
    context.AddRange(plugins);
    await context.SaveChangesAsync();

    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    var summaries = await pluginService.ListPlugins("*", new Pageable(1, 10));
    Assert.That(summaries, Has.Count.EqualTo(10));


    summaries = await pluginService.ListPlugins("Plugin1", new Pageable(1, 10));
    Assert.That(summaries, Has.Count.EqualTo(1));
    Assert.That(summaries[0].Name, Is.EqualTo("Plugin1"));

    summaries = await pluginService.ListPlugins("Plugin2", new Pageable(1, 10));
    Assert.That(summaries, Has.Count.EqualTo(1));
    Assert.That(summaries[0].Name, Is.EqualTo("Plugin2"));
  }

  [Test]
  public async Task TestAddPlugins() {
    var dummy = _mockFilesystem.FileInfo.New("Dummy");
    var partitioned = new PartitionedPlugin(new ResourceHandle("Source", dummy), null, null, []);

    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    var plugin1 = await pluginService.AddPlugin("Plugin1", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(1, 0, 0)
    }, partitioned);

    var plugin2 = await pluginService.AddPlugin("Plugin2", new PluginDescriptor {
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
    }, partitioned);

    var plugin3 = await pluginService.AddPlugin("Plugin3", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(1, 0, 0),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "Plugin2",
                PluginType = PluginType.Provided
            }
        ]
    }, partitioned);

    await pluginService.AddPlugin("Plugin3", new PluginDescriptor {
        Version = 1,
        VersionName = new SemVersion(1, 2, 1),
        Plugins = [
            new PluginReferenceDescriptor {
                Name = "Plugin2",
                PluginType = PluginType.Provided
            }
        ]
    }, partitioned);

    var plugin1List = await pluginService.GetDependencyList(plugin1.PluginId);
    Assert.That(plugin1List, Has.Count.EqualTo(1));
    Assert.That(plugin1List[0].Name, Is.EqualTo("Plugin1"));

    var plugin2List = await pluginService.GetDependencyList(plugin2.PluginId);
    Assert.That(plugin2List, Has.Count.EqualTo(2));
    var plugin2Names = plugin2List.Select(x => x.Name).ToList();
    Assert.That(plugin2Names, Does.Contain("Plugin1"));
    Assert.That(plugin2Names, Does.Contain("Plugin2"));

    var plugin3List = await pluginService.GetDependencyList(plugin3.PluginId);
    Assert.That(plugin3List, Has.Count.EqualTo(3));
    var plugin3Names = plugin3List.Select(x => x.Name).ToList();
    Assert.That(plugin3Names, Does.Contain("Plugin1"));
    Assert.That(plugin3Names, Does.Contain("Plugin2"));
    Assert.That(plugin3Names, Does.Contain("Plugin3"));
  }

  [Test]
  public async Task TestAddPluginVersions() {
    var dummy = _mockFilesystem.FileInfo.New("Dummy");
    var partitioned = new PartitionedPlugin(new ResourceHandle("Source", dummy), null, null, []);
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    var app = await pluginService.SetupVersionResolutionTree(partitioned);

    var dependencyGraph = await pluginService.GetDependencyList(app);
    Assert.That(dependencyGraph, Has.Count.EqualTo(5));
    Assert.Multiple(() => {
      Assert.That(dependencyGraph.Find(x => x.Name == "Threads")?.Version, Is.EqualTo(new SemVersion(2, 0, 0)));
      Assert.That(dependencyGraph.Find(x => x.Name == "StdLib")?.Version, Is.EqualTo(new SemVersion(4, 0, 0)));
      Assert.That(dependencyGraph.Find(x => x.Name == "Sql")?.Version, Is.EqualTo(new SemVersion(2, 0, 0)));
      Assert.That(dependencyGraph.Find(x => x.Name == "Http")?.Version, Is.EqualTo(new SemVersion(4, 0, 0)));
      Assert.That(dependencyGraph.Find(x => x.Name == "App")?.Version, Is.EqualTo(new SemVersion(1, 0, 0)));
    });
  }

  [Test]
  public async Task TestAddPluginVersionsWithPreInstalledPlugins() {
    var dummy = _mockFilesystem.FileInfo.New("Dummy");
    var partitioned = new PartitionedPlugin(new ResourceHandle("Source", dummy), null, null, []);
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    await pluginService.SetupVersionResolutionTree(partitioned);

    List<PluginDependency> pluginDependencies = [
        new() {
            PluginName = "Sql",
            Type = PluginType.Provided,
            PluginVersion = SemVersionRange.Parse("=2.0.0")
        },
        new() {
            PluginName = "Threads",
            Type = PluginType.Provided,
            PluginVersion = SemVersionRange.Parse("=2.0.0")
        },
        new() {
            PluginName = "Http",
            Type = PluginType.Provided,
            PluginVersion = SemVersionRange.Parse(">=3.0.0 <=4.0.0")
        },
        new() {
            PluginName = "StdLib",
            Type = PluginType.Provided,
            PluginVersion = SemVersionRange.Parse("=4.0.0")
        }
    ];
    var possibleVersions = await pluginService.GetPossibleVersions(pluginDependencies);
    Assert.That(possibleVersions.FoundDependencies, Contains.Key("Http"));
    var http3 = possibleVersions.FoundDependencies["Http"]
        .Find(x => x.Version.Major == 3);
    Assert.That(http3, Is.Not.Null);
    http3.Installed = true;

    var root = new DependencyChainRoot {
        Dependencies = pluginDependencies
    };

    var dependencyGraph = pluginService.GetDependencyList(root, possibleVersions);
    Assert.That(dependencyGraph, Has.Count.EqualTo(4));
    Assert.Multiple(() => {
      Assert.That(dependencyGraph.Find(x => x.Name == "Threads")?.Version, Is.EqualTo(new SemVersion(2, 0, 0)));
      Assert.That(dependencyGraph.Find(x => x.Name == "StdLib")?.Version, Is.EqualTo(new SemVersion(4, 0, 0)));
      Assert.That(dependencyGraph.Find(x => x.Name == "Sql")?.Version, Is.EqualTo(new SemVersion(2, 0, 0)));
      Assert.That(dependencyGraph.Find(x => x.Name == "Http")?.Version, Is.EqualTo(new SemVersion(3, 0, 0)));
    });
  }

  [Test]
  public async Task TestGetDependencyTreeWithConflicts() {
    var dummy = _mockFilesystem.FileInfo.New("Dummy");
    var partitioned = new PartitionedPlugin(new ResourceHandle("Source", dummy), null, null, []);
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    var app = await pluginService.SetupVersionResolutionTreeWithConflict(partitioned);

    var conflictException =
        Assert.ThrowsAsync<DependencyConflictException>(async () => await pluginService.GetDependencyList(app));
    Assert.That(conflictException, Is.Not.Null);
    var conflicts = conflictException.Conflicts;
    Assert.That(conflicts, Has.Count.EqualTo(1));
    Assert.Multiple(() => {
      Assert.That(conflicts[0].PluginName, Is.EqualTo("ConflictingDependency"));
      Assert.That(conflicts[0].Versions, Has.Count.EqualTo(2));
    });
    Assert.That(conflicts[0].Versions,
        Has.Exactly(1)
            .Matches<PluginRequirement>(x => x.RequiredBy == "App"
                                             && x.RequiredVersion == SemVersionRange.Parse("=1.0.0")));
    Assert.That(conflicts[0].Versions,
        Has.Exactly(1)
            .Matches<PluginRequirement>(x => x.RequiredBy == "Sql"
                                             && x.RequiredVersion == SemVersionRange.Parse("=2.0.0")));
  }

  [Test]
  public async Task TestSubmitPlugin() {
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    using var testZip = new MemoryStream();
    using (var zipArchive = new ZipArchive(testZip, ZipArchiveMode.Create, true)) {
      zipArchive.CreateEntry("Resources/");
      zipArchive.CreateEntry("Resources/Icon128.png");
      zipArchive.CreateEntry("Binaries/");
      zipArchive.CreateEntry("Binaries/Win64/");
      zipArchive.CreateEntry("Binaries/Win64/TestPlugin.dll");

      var entry = zipArchive.CreateEntry("TestPlugin.uplugin");
      await using var writer = new StreamWriter(entry.Open());

      var descriptor = new PluginDescriptor {
          FriendlyName = "Test Plugin",
          VersionName = new SemVersion(1, 0, 0),
          Description = "Test description"
      };

      await writer.WriteAsync(JsonSerializer.Serialize(descriptor, JsonOptions));
    }

    testZip.Seek(0, SeekOrigin.Begin);

    var summary = await pluginService.SubmitPlugin(testZip, "5.5");
    Assert.Multiple(() => {
      Assert.That(summary.Name, Is.EqualTo("TestPlugin"));
      Assert.That(summary.Description, Is.EqualTo("Test description"));
      Assert.That(summary.Version, Is.EqualTo(new SemVersion(1, 0, 0)));
    });
  }

  [Test]
  public async Task TestSubmitPluginFromDirectory() {
    var filesystem = _serviceProvider.GetRequiredService<IFileSystem>();
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();

    var tempDir = filesystem.Directory.CreateDirectory("TestPlugin");
    filesystem.Directory.CreateDirectory(Path.Combine(tempDir.FullName, "Resources"));
    filesystem.Directory.CreateDirectory(Path.Combine(tempDir.FullName, "Binaries"));
    filesystem.Directory.CreateDirectory(Path.Combine(tempDir.FullName, "Binaries", "Win64"));
    filesystem.Directory.CreateDirectory(Path.Combine(tempDir.FullName, "Intermediate"));
    filesystem.Directory.CreateDirectory(Path.Combine(tempDir.FullName, "Intermediate", "Build"));
    filesystem.Directory.CreateDirectory(Path.Combine(tempDir.FullName, "Intermediate", "Build", "Win64"));
    await filesystem.File.Create(Path.Combine(tempDir.FullName, "Resources", "Icon128.png")).DisposeAsync();
    await filesystem.File.Create(Path.Combine(tempDir.FullName, "Binaries", "Win64", "TestPlugin.dll")).DisposeAsync();
    await filesystem.File.Create(Path.Combine(tempDir.FullName, "Intermediate", "Build", "Win64", "TestPlugin.lib"))
        .DisposeAsync();
    await using (var upluginFile = filesystem.File.Create(Path.Combine(tempDir.FullName, "TestPlugin.uplugin"))) {
      await using var writer = new StreamWriter(upluginFile);

      var descriptor = new PluginDescriptor {
          FriendlyName = "Test Plugin",
          VersionName = new SemVersion(1, 0, 0),
          Description = "Test description"
      };

      await writer.WriteAsync(JsonSerializer.Serialize(descriptor, JsonOptions));
    }

    var summary = await pluginService.SubmitPlugin(tempDir, "5.5");
    Assert.Multiple(() => {
      Assert.That(summary.Name, Is.EqualTo("TestPlugin"));
      Assert.That(summary.Description, Is.EqualTo("Test description"));
      Assert.That(summary.Version, Is.EqualTo(new SemVersion(1, 0, 0)));
    });
  }

  [Test]
  public async Task TestSubmitPlugin_MalformedDescriptor() {
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    using var testZip = new MemoryStream();
    using (var zipArchive = new ZipArchive(testZip, ZipArchiveMode.Create, true)) {
      var entry = zipArchive.CreateEntry("TestPlugin.uplugin");
      await using var writer = new StreamWriter(entry.Open());

      await writer.WriteAsync("This is not a JSON file");
    }

    Assert.ThrowsAsync<BadSubmissionException>(async () =>
        await pluginService.SubmitPlugin(testZip, "5.5"));
  }

  [Test]
  public async Task TestSubmitPlugin_NoUpluginFile() {
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    using var testZip = new MemoryStream();
    using (var zipArchive = new ZipArchive(testZip, ZipArchiveMode.Create, true)) {
      var entry = zipArchive.CreateEntry("TestPlugin.json");
      await using var writer = new StreamWriter(entry.Open());

      var descriptor = new PluginDescriptor {
          FriendlyName = "Test Plugin",
          VersionName = new SemVersion(1, 0, 0),
          Description = "Test description"
      };

      await writer.WriteAsync(JsonSerializer.Serialize(descriptor, JsonOptions));
    }

    Assert.ThrowsAsync<BadSubmissionException>(async () =>
        await pluginService.SubmitPlugin(testZip, "5.5"));
  }

  [Test]
  public async Task TestSubmitPluginAsParts() {
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    using var testZip = new MemoryStream();
    using (var zipArchive = new ZipArchive(testZip, ZipArchiveMode.Create, true)) {
      zipArchive.CreateEntry("Icon.png");
      zipArchive.CreateEntry("Binaries/");
      zipArchive.CreateEntry("Binaries/5.5/");
      zipArchive.CreateEntry("Binaries/5.5/Win64.zip");

      using var innerZip = new MemoryStream();
      using (var innerArchive = new ZipArchive(innerZip, ZipArchiveMode.Create, true)) {
        var entry = innerArchive.CreateEntry("TestPlugin.uplugin");
        await using var writer = new StreamWriter(entry.Open());

        var descriptor = new PluginDescriptor {
            FriendlyName = "Test Plugin",
            VersionName = new SemVersion(1, 0, 0),
            Description = "Test description"
        };

        await writer.WriteAsync(JsonSerializer.Serialize(descriptor, JsonOptions));
      }

      var sourceEntry = zipArchive.CreateEntry("Source.zip");
      await using var sourceWriter = sourceEntry.Open();
      innerZip.Seek(0, SeekOrigin.Begin);
      await innerZip.CopyToAsync(sourceWriter);
    }

    testZip.Seek(0, SeekOrigin.Begin);

    var summary = await pluginService.SubmitPlugin(testZip);
    Assert.Multiple(() => {
      Assert.That(summary.Name, Is.EqualTo("TestPlugin"));
      Assert.That(summary.Description, Is.EqualTo("Test description"));
      Assert.That(summary.Version, Is.EqualTo(new SemVersion(1, 0, 0)));
    });
  }

  [Test]
  public async Task TestRetrievePlugin() {
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    var (pluginId, _) = await SetupTestPluginEnvironment();
    _context.ChangeTracker.Clear();

    Assert.DoesNotThrowAsync(
        async () => await pluginService.GetPluginFileData(pluginId, SemVersionRange.All, "5.5", ["Win64"]));
    _context.ChangeTracker.Clear();
    Assert.ThrowsAsync<PluginNotFoundException>(async () =>
        await pluginService.GetPluginFileData(
            pluginId, SemVersionRange.All, "5.4", ["Win64"]));
    _context.ChangeTracker.Clear();
    Assert.ThrowsAsync<PluginNotFoundException>(async () =>
        await pluginService.GetPluginFileData(
            Guid.NewGuid(), SemVersionRange.All, "5.5", ["Win64"]));
  }

  [Test]
  public async Task TestRetrievePluginParts() {
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    var (pluginId, versionId) = await SetupTestPluginEnvironment();
    _context.ChangeTracker.Clear();

    Assert.DoesNotThrowAsync(
        async () => await pluginService.GetPluginFileData(pluginId, versionId));
    _context.ChangeTracker.Clear();
    Assert.ThrowsAsync<PluginNotFoundException>(async () =>
        await pluginService.GetPluginFileData(
            pluginId, SemVersionRange.All, "5.4", ["Win64"]));
    _context.ChangeTracker.Clear();
    Assert.ThrowsAsync<PluginNotFoundException>(async () =>
        await pluginService.GetPluginFileData(
            Guid.NewGuid(), SemVersionRange.All, "5.5", ["Win64"]));
  }

  [Test]
  public async Task TestAddAndUpdateReadme() {
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    var (pluginId, versionId) = await SetupTestPluginEnvironment();

    Assert.ThrowsAsync<PluginNotFoundException>(() => pluginService.GetPluginReadme(pluginId, versionId));

    const string readmeText = "This is a readme";
    Assert.ThrowsAsync<PluginNotFoundException>(() =>
        pluginService.UpdatePluginReadme(Guid.NewGuid(), versionId, readmeText));
    Assert.ThrowsAsync<BadSubmissionException>(() => pluginService.UpdatePluginReadme(pluginId, versionId, readmeText));

    var added = await pluginService.AddPluginReadme(pluginId, versionId, readmeText);
    Assert.ThrowsAsync<PluginNotFoundException>(() =>
        pluginService.AddPluginReadme(Guid.NewGuid(), versionId, readmeText));
    Assert.ThrowsAsync<BadSubmissionException>(() => pluginService.AddPluginReadme(pluginId, versionId, readmeText));
    Assert.That(added, Is.EqualTo(readmeText));

    var retrieved = await pluginService.GetPluginReadme(pluginId, versionId);
    Assert.That(retrieved, Is.EqualTo(readmeText));

    const string updatedReadmeText = "This is an updated readme";
    var updated = await pluginService.UpdatePluginReadme(pluginId, versionId, updatedReadmeText);
    Assert.That(updated, Is.EqualTo(updatedReadmeText));

    retrieved = await pluginService.GetPluginReadme(pluginId, versionId);
    Assert.That(retrieved, Is.EqualTo(updatedReadmeText));
  }

  private async Task<(Guid, Guid)> SetupTestPluginEnvironment() {

    var filesystem = _serviceProvider.GetRequiredService<IFileSystem>();
    var context = _serviceProvider.GetRequiredService<UnrealPluginManagerContext>();

    var tempFileName = Path.GetTempFileName();
    var dirName = Path.GetDirectoryName(tempFileName);
    Assert.That(dirName, Is.Not.Null);
    filesystem.Directory.CreateDirectory(dirName);

    var zipFile = filesystem.FileInfo.New("TestPlugin.zip");
    await using var testZip = zipFile.Create();
    using (var zipArchive = new ZipArchive(testZip, ZipArchiveMode.Create)) {
      var entry = zipArchive.CreateEntry("TestPlugin.json");
      await using var writer = new StreamWriter(entry.Open());

      var descriptor = new PluginDescriptor {
          FriendlyName = "Test Plugin",
          VersionName = new SemVersion(1, 0, 0),
          Description = "Test description"
      };

      await writer.WriteAsync(JsonSerializer.Serialize(descriptor, JsonOptions));
    }

    var binaries = filesystem.FileInfo.New("Binaries.zip");
    await using var binZip = binaries.Create();
    using (var zipArchive = new ZipArchive(binZip, ZipArchiveMode.Create)) {
      var entry = zipArchive.CreateEntry("TestPlugin.dll");
      await using var writer = new StreamWriter(entry.Open());
      await writer.WriteAsync("TestPlugin.dll");
    }

    var (zipName, _) = await _storageService.AddResource(new CopyFileSource(zipFile));
    var (binName, _) = await _storageService.AddResource(new CopyFileSource(binaries));

    var plugin = new Plugin {
        Name = "TestPlugin",
        FriendlyName = "Test Plugin",

        Description = "Test description",
        Versions = [
            new PluginVersion {
                Version = new SemVersion(1, 0, 0),
                Source = new FileResource {
                    OriginalFilename = "Source.zip",
                    StoredFilename = zipName,
                },
                Binaries = [
                    new UploadedBinaries {
                        EngineVersion = "5.5",
                        Platform = "Win64",
                        File = new FileResource {
                            OriginalFilename = "Win64.zip",
                            StoredFilename = binName
                        }
                    }
                ]
            }
        ]
    };
    await context.Plugins.AddAsync(plugin);
    await context.SaveChangesAsync();

    return (plugin.Id, plugin.Versions.First().Id);
  }
}