using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Compression;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Retro.SimplePage;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Core.Model.Resolution;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Tests.Database;
using UnrealPluginManager.Core.Tests.Helpers;
using UnrealPluginManager.Core.Tests.Mocks;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Tests.Services;

public class PluginServiceTests {
  private static readonly JsonSerializerOptions JsonOptions = new() {
      AllowTrailingCommas = true
  };

  private MockFileSystem _mockFilesystem;
  private UnrealPluginManagerContext _context;
  private ServiceProvider _serviceProvider;

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

    _serviceProvider.GetRequiredService<IStorageService>();
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
                Versions = [
                    new PluginVersion {
                        Version = new SemVersion(1, 0, 0),
                        Source = new SourceLocation {
                            Url = new Uri("https://github.com/ue4plugins/TestPlugin"),
                            Sha = "Dummy"
                        }
                    },
                    new PluginVersion {
                        Version = new SemVersion(1, 2, 2),
                        Source = new SourceLocation {
                            Url = new Uri("https://github.com/ue4plugins/TestPlugin"),
                            Sha = "Dummy"
                        }
                    }
                ]
            }
        });
    context.AddRange(plugins);
    await context.SaveChangesAsync();

    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    var summaries = await pluginService.ListPlugins("", new Pageable(1, 10));
    Assert.That(summaries, Has.Count.EqualTo(10));


    summaries = await pluginService.ListPlugins("Plugin1", new Pageable(1, 10));
    Assert.That(summaries, Has.Count.EqualTo(2));
    Assert.Multiple(() => {
      Assert.That(summaries[0].Name, Is.EqualTo("Plugin10"));
      Assert.That(summaries[1].Name, Is.EqualTo("Plugin1"));
    });

    summaries = await pluginService.ListPlugins("Plugin2", new Pageable(1, 10));
    Assert.That(summaries, Has.Count.EqualTo(1));
    Assert.That(summaries[0].Name, Is.EqualTo("Plugin2"));
  }

  [Test]
  public async Task TestAddPlugins() {
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    var plugin1 = await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Plugin1",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Plugin1"),
            Sha = "Plugin1TestSha"
        },
        Dependencies = []
    }, []);

    var plugin2 = await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Plugin2",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Plugin2"),
            Sha = "Plugin2TestSha"
        },
        Dependencies = [
            new PluginDependencyManifest {
                Name = "Plugin1",
                Version = SemVersionRange.Parse(">=1.0.0")
            }
        ]
    }, []);

    var plugin3 = await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Plugin3",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Plugin3"),
            Sha = "Plugin3TestSha"
        },
        Dependencies = [
            new PluginDependencyManifest {
                Name = "Plugin2",
                Version = SemVersionRange.AtLeast(new SemVersion(1, 0, 0))
            }
        ]
    }, []);

    await pluginService.SubmitPlugin(new PluginManifest {
        Name = "Plugin3",
        Version = new SemVersion(1, 2, 1),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/Plugin3/v1.2.1"),
            Sha = "Plugin3V121TestSha"
        },
        Dependencies = [
            new PluginDependencyManifest {
                Name = "Plugin2",
                Version = SemVersionRange.AtLeast(new SemVersion(1, 0, 0))
            }
        ]
    }, []);

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
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    var app = await pluginService.SetupVersionResolutionTree();

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
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    await pluginService.SetupVersionResolutionTree();

    List<PluginDependency> pluginDependencies = [
        new() {
            PluginName = "Sql",
            PluginVersion = SemVersionRange.Parse("=2.0.0")
        },
        new() {
            PluginName = "Threads",
            PluginVersion = SemVersionRange.Parse("=2.0.0")
        },
        new() {
            PluginName = "Http",
            PluginVersion = SemVersionRange.Parse(">=3.0.0 <=4.0.0")
        },
        new() {
            PluginName = "StdLib",
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
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    var app = await pluginService.SetupVersionResolutionTreeWithConflict();

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
  public async Task TestAddAndUpdateReadme() {
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
    var (pluginId, versionId) = await SetupTestPluginEnvironment();

    Assert.ThrowsAsync<PluginNotFoundException>(() => pluginService.GetPluginReadme(pluginId, versionId));

    const string readmeText = "This is a readme";
    Assert.ThrowsAsync<PluginNotFoundException>(() =>
                                                    pluginService.UpdatePluginReadme(
                                                        Guid.NewGuid(), versionId, readmeText));
    Assert.ThrowsAsync<BadSubmissionException>(() => pluginService.UpdatePluginReadme(pluginId, versionId, readmeText));

    var added = await pluginService.AddPluginReadme(pluginId, versionId, readmeText);
    Assert.ThrowsAsync<PluginNotFoundException>(() =>
                                                    pluginService.AddPluginReadme(
                                                        Guid.NewGuid(), versionId, readmeText));
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

  [Test]
  public async Task TestSubmitPluginWithStream_Success() {
    // Arrange
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();

    // Create the necessary files for a valid plugin archive
    using var tempDir = _mockFilesystem.CreateDisposableDirectory(out var directory);

    // Create a plugin.json file
    var pluginManifest = new PluginManifest {
        Name = "StreamTestPlugin",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/StreamTestPlugin"),
            Sha = "StreamTestSha"
        },
        Dependencies = [],
        Patches = ["patch1.txt", "patch2.txt"]
    };

    // Create plugin.json
    var manifestFile = _mockFilesystem.FileInfo.New(Path.Join(directory.FullName, "plugin.json"));
    await using (var manifestStream = manifestFile.Create()) {
      var pluginManifestJson = _serviceProvider.GetRequiredService<IJsonService>().Serialize(pluginManifest);
      await using var pluginManifestStream = pluginManifestJson.ToStream();
      await pluginManifestStream.CopyToAsync(manifestStream);
    }

    // Create patches directory
    var patchesDir = _mockFilesystem.DirectoryInfo.New(Path.Join(directory.FullName, "patches"));
    patchesDir.Create();

    // Create patch files
    var patch1File = _mockFilesystem.FileInfo.New(Path.Join(patchesDir.FullName, "patch1.txt"));
    await using (var patch1Stream = patch1File.Create()) {
      await using var writer = new StreamWriter(patch1Stream);
      await writer.WriteLineAsync("This is patch 1");
    }

    var patch2File = _mockFilesystem.FileInfo.New(Path.Join(patchesDir.FullName, "patch2.txt"));
    await using (var patch2Stream = patch2File.Create()) {
      await using var writer = new StreamWriter(patch2Stream);
      await writer.WriteLineAsync("This is patch 2");
    }

    // Create an icon
    var iconFile = _mockFilesystem.FileInfo.New(Path.Join(directory.FullName, "icon.png"));
    await using (var iconStream = iconFile.Create()) {
      await using var writer = new StreamWriter(iconStream);
      await writer.WriteLineAsync("Dummy icon data");
    }

    // Create a README
    var readmeFile = _mockFilesystem.FileInfo.New(Path.Join(directory.FullName, "README.md"));
    await using (var readmeStream = readmeFile.Create()) {
      await using var writer = new StreamWriter(readmeStream);
      await writer.WriteLineAsync("# Test Plugin\nThis is a test plugin.");
    }

    // Create a zip file from the directory
    Assert.That(directory.Parent, Is.Not.Null);
    var zipFilePath = Path.Join(directory.Parent.FullName, "plugin.zip");
    var zipFile = _mockFilesystem.FileInfo.New(zipFilePath);

    await using (var zipStream = zipFile.Create()) {
      using var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true);

      // Add all files to the zip
      foreach (var file in _mockFilesystem.Directory.GetFiles(directory.FullName, "*", SearchOption.AllDirectories)) {
        var relativePath = file[(directory.FullName.Length + 1)..].Replace(Path.PathSeparator, '/');
        await _mockFilesystem.CreateEntryFromFile(zipArchive, file, relativePath);
      }
    }

    // Act
    await using var archiveStream = zipFile.OpenRead();
    var result = await pluginService.SubmitPlugin(archiveStream);

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.Multiple(() => {
      Assert.That(result.Name, Is.EqualTo("StreamTestPlugin"));
      Assert.That(result.Version, Is.EqualTo(new SemVersion(1, 0, 0)));
      Assert.That(result.Dependencies, Is.Empty);
      Assert.That(result.Patches, Has.Count.EqualTo(2));
    });

    // Check that the plugin was actually added to the database
    var pluginVersions = await _context.PluginVersions
        .Include(v => v.Parent)
        .Include(v => v.Patches)
        .ThenInclude(p => p.FileResource)
        .Include(v => v.Icon)
        .Include(v => v.Readme)
        .Where(v => v.Parent.Name == "StreamTestPlugin")
        .ToListAsync();

    Assert.That(pluginVersions, Has.Count.EqualTo(1));
    var version = pluginVersions[0];
    Assert.Multiple(() => {
      Assert.That(version.Parent.Name, Is.EqualTo("StreamTestPlugin"));
      Assert.That(version.Version, Is.EqualTo(new SemVersion(1, 0, 0)));
      Assert.That(version.Patches, Has.Count.EqualTo(2));
      Assert.That(version.Icon, Is.Not.Null);
      Assert.That(version.Readme, Is.Not.Null);
    });
  }

  [Test]
  public async Task TestSubmitPluginWithStream_MissingManifest() {
    // Arrange
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();

    // Create directory with no plugin.json file
    using var tempDir = _mockFilesystem.CreateDisposableDirectory(out var directory);

    // Create a dummy file (but no plugin.json)
    var dummyFile = _mockFilesystem.FileInfo.New(Path.Join(directory.FullName, "dummy.txt"));
    await using (var dummyStream = dummyFile.Create()) {
      await using var writer = new StreamWriter(dummyStream);
      await writer.WriteLineAsync("This is a dummy file");
    }

    // Create a zip file from the directory
    Assert.That(directory.Parent, Is.Not.Null);
    var zipFilePath = Path.Join(directory.Parent.FullName, "plugin.zip");
    var zipFile = _mockFilesystem.FileInfo.New(zipFilePath);

    await using (var zipStream = zipFile.Create()) {
      using var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true);

      // Add all files to the zip
      foreach (var file in _mockFilesystem.Directory.GetFiles(directory.FullName, "*", SearchOption.AllDirectories)) {
        var relativePath = file[(directory.FullName.Length + 1)..].Replace(Path.PathSeparator, '/');
        await _mockFilesystem.CreateEntryFromFile(zipArchive, file, relativePath);
      }
    }

    // Act & Assert
    await using var archiveStream = zipFile.OpenRead();
    var exception = Assert.ThrowsAsync<BadSubmissionException>(() => pluginService.SubmitPlugin(archiveStream));
    Assert.That(exception.Message, Does.Contain("Plugin manifest file was not found"));
  }

  [Test]
  public async Task TestSubmitPluginWithStream_MissingPatchFiles() {
    // Arrange
    var pluginService = _serviceProvider.GetRequiredService<IPluginService>();

    // Create the necessary files for plugin archive with missing patches
    using var tempDir = _mockFilesystem.CreateDisposableDirectory(out var directory);

    // Create a plugin.json file referencing patches that don't exist
    var pluginManifest = new PluginManifest {
        Name = "MissingPatchesPlugin",
        Version = new SemVersion(1, 0, 0),
        Source = new SourceLocation {
            Url = new Uri("https://github.com/ue4plugins/MissingPatchesPlugin"),
            Sha = "MissingPatchesTestSha"
        },
        Dependencies = [],
        Patches = ["missing_patch.txt"]
    };

    // Create plugin.json
    var manifestFile = _mockFilesystem.FileInfo.New(Path.Join(directory.FullName, "plugin.json"));
    await using (var manifestStream = manifestFile.Create()) {
      var pluginManifestJson = _serviceProvider.GetRequiredService<IJsonService>().Serialize(pluginManifest);
      await using var pluginManifestStream = pluginManifestJson.ToStream();
      await pluginManifestStream.CopyToAsync(manifestStream);
    }

    // Create patches directory but don't add the referenced patch file
    var patchesDir = _mockFilesystem.DirectoryInfo.New(Path.Join(directory.FullName, "patches"));
    patchesDir.Create();

    // Create a zip file from the directory
    Assert.That(directory.Parent, Is.Not.Null);
    var zipFilePath = Path.Join(directory.Parent.FullName, "plugin.zip");
    var zipFile = _mockFilesystem.FileInfo.New(zipFilePath);

    await using (var zipStream = zipFile.Create()) {
      using var zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, true);

      // Add all files to the zip
      foreach (var file in _mockFilesystem.Directory.GetFiles(directory.FullName, "*", SearchOption.AllDirectories)) {
        var relativePath = file[(directory.FullName.Length + 1)..].Replace(Path.PathSeparator, '/');
        await _mockFilesystem.CreateEntryFromFile(zipArchive, file, relativePath);
      }
    }

    // Act & Assert
    await using var archiveStream = zipFile.OpenRead();
    var exception = Assert.ThrowsAsync<BadSubmissionException>(() => pluginService.SubmitPlugin(archiveStream));
    Assert.That(exception.Message, Does.Contain("Missing patch file"));
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

    var plugin = new Plugin {
        Name = "TestPlugin",
        Versions = [
            new PluginVersion {
                Version = new SemVersion(1, 0, 0),
                Source = new SourceLocation {
                    Url = new Uri("https://github.com/ue4plugins/TestPlugin"),
                    Sha = "Dummy"
                }
            }
        ]
    };
    await context.Plugins.AddAsync(plugin);
    await context.SaveChangesAsync();

    return (plugin.Id, plugin.Versions.First().Id);
  }
}