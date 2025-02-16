using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Compression;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Pagination;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Tests.Database;
using UnrealPluginManager.Core.Tests.Helpers;

namespace UnrealPluginManager.Core.Tests.Services;

public class PluginServiceTests {
    private static readonly JsonSerializerOptions JsonOptions = new() {
        AllowTrailingCommas = true
    };

    private ServiceProvider _serviceProvider;
    private Mock<IStorageService> _mockStorageService;

    [SetUp]
    public void Setup() {
        var services = new ServiceCollection();

        var mockFilesystem = new MockFileSystem();
        services.AddSingleton<IFileSystem>(mockFilesystem);

        _mockStorageService = new Mock<IStorageService>();
        services.AddSingleton(_mockStorageService.Object);


        services.AddDbContext<UnrealPluginManagerContext, TestUnrealPluginManagerContext>();
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
            .SelectMany(i => new[] {
                new Plugin {
                    Name = "Plugin" + i,
                    FriendlyName = "Plugin" + i,
                    Versions = [
                        new PluginVersion {
                            Version = new SemVersion(1, 0, 0)
                        },
                        new PluginVersion {
                            Version = new SemVersion(1, 2, 2)
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

    [Test]
    public async Task TestSubmitPlugin() {
        var filesystem = _serviceProvider.GetRequiredService<IFileSystem>();
        var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
        using var testZip = new MemoryStream();
        using (var zipArchive = new ZipArchive(testZip, ZipArchiveMode.Create, true)) {
            var entry = zipArchive.CreateEntry("TestPlugin.uplugin");
            await using var writer = new StreamWriter(entry.Open());

            var descriptor = new PluginDescriptor {
                FriendlyName = "Test Plugin",
                VersionName = new SemVersion(1, 0, 0),
                Description = "Test description"
            };

            await writer.WriteAsync(JsonSerializer.Serialize(descriptor, JsonOptions));
        }

        _mockStorageService.Setup(x => x.StorePlugin(It.IsAny<Stream>()))
            .Returns(async (Stream input) => {
                var dummyInfo = filesystem.FileInfo.New("dummyFile.zip");
                await using var stream = dummyInfo.Create();
                input.Seek(0, SeekOrigin.Begin);
                await input.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return dummyInfo;
            });

        var summary = await pluginService.SubmitPlugin(testZip, new Version(5, 5));
        Assert.Multiple(() => {
            Assert.That(summary.Name, Is.EqualTo("TestPlugin"));
            Assert.That(summary.Version, Is.EqualTo(new SemVersion(1, 0, 0)));
            Assert.That(summary.Description, Is.EqualTo("Test description"));
        });
    }

    [Test]
    public async Task TestSubmitPlugin_MalformedDescriptor() {
        var filesystem = _serviceProvider.GetRequiredService<IFileSystem>();
        var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
        using var testZip = new MemoryStream();
        using (var zipArchive = new ZipArchive(testZip, ZipArchiveMode.Create, true)) {
            var entry = zipArchive.CreateEntry("TestPlugin.uplugin");
            await using var writer = new StreamWriter(entry.Open());

            await writer.WriteAsync("This is not a JSON file");
        }

        _mockStorageService.Setup(x => x.StorePlugin(It.IsAny<Stream>()))
            .Returns(async (Stream input) => {
                var dummyInfo = filesystem.FileInfo.New("dummyFile.zip");
                await using var stream = dummyInfo.Create();
                input.Seek(0, SeekOrigin.Begin);
                await input.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return dummyInfo;
            });

        Assert.ThrowsAsync<BadSubmissionException>(async () =>
            await pluginService.SubmitPlugin(testZip, new Version(5, 5)));
    }

    [Test]
    public async Task TestSubmitPlugin_NoUpluginFile() {
        var filesystem = _serviceProvider.GetRequiredService<IFileSystem>();
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

        _mockStorageService.Setup(x => x.StorePlugin(It.IsAny<Stream>()))
            .Returns(async (Stream input) => {
                var dummyInfo = filesystem.FileInfo.New("dummyFile.zip");
                await using var stream = dummyInfo.Create();
                input.Seek(0, SeekOrigin.Begin);
                await input.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                return dummyInfo;
            });

        Assert.ThrowsAsync<BadSubmissionException>(async () =>
            await pluginService.SubmitPlugin(testZip, new Version(5, 5)));
    }

    [Test]
    public async Task TestRetrievePlugin() {
        var filesystem = _serviceProvider.GetRequiredService<IFileSystem>();
        var pluginService = _serviceProvider.GetRequiredService<IPluginService>();
        var context = _serviceProvider.GetRequiredService<UnrealPluginManagerContext>();

        var zipFile = filesystem.FileInfo.New("TestPlugin.zip");
        await using var testZip = zipFile.Create();
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

        var plugin = new Plugin {
            Name = "TestPlugin",
            FriendlyName = "Test Plugin",
            
            Description = "Test description",
            Versions = [
                new PluginVersion {
                    Version = new SemVersion(1, 0, 0),
                    Binaries = [
                        new PluginBinaries {
                            EngineVersion = "5.5",
                            FilePath = zipFile,
                            Platform = "Win64"
                        }
                    ]
                }
            ]
        };
        await context.Plugins.AddAsync(plugin);
        await context.SaveChangesAsync();

        Assert.DoesNotThrowAsync(async () => await pluginService.GetPluginFileData("TestPlugin", SemVersionRange.All, "5.5"));
        Assert.ThrowsAsync<PluginNotFoundException>(async () =>
            await pluginService.GetPluginFileData("TestPlugin", SemVersionRange.All, "5.4"));
        Assert.ThrowsAsync<PluginNotFoundException>(async () =>
            await pluginService.GetPluginFileData("OtherPlugin", SemVersionRange.All, "5.5"));
    }
}