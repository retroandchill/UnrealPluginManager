using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Compression;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Semver;
using UnrealPluginManager.Cli.Model.Engine;
using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Core.Abstractions;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Cli.Tests.Services;

public partial class EngineServiceTest {
    
    private static readonly JsonSerializerOptions JsonOptions = new() {
        AllowTrailingCommas = true
    };
    
    private ServiceProvider _serviceProvider;
    private MockFileSystem _filesystem;
    private Mock<IEnginePlatformService> _enginePlatformService;
    private Mock<IProcessRunner> _processRunner;
    private Mock<IPluginService> _pluginService;
    
    [SetUp]
    public void Setup() {
        var services = new ServiceCollection();

        _filesystem = new MockFileSystem(new Dictionary<string, MockFileData>());
        services.AddSingleton<IFileSystem>(_filesystem);

        _enginePlatformService = new Mock<IEnginePlatformService>();
        services.AddSingleton(_enginePlatformService.Object);
        _processRunner = new Mock<IProcessRunner>();
        services.AddSingleton(_processRunner.Object);
        _pluginService = new Mock<IPluginService>();
        services.AddSingleton(_pluginService.Object);
        
        services.AddSingleton<IEngineService, EngineService>();
        
        _serviceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public void TearDown() {
        _serviceProvider.Dispose();
    }

    [Test]
    public async Task TestBuildPlugin() {
        var installedEngines = new List<InstalledEngine> {
            new("5.4", new Version(5, 4), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.4")),
            new("5.5", new Version(5, 5), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.5")),
            new("5.6_Custom", new Version(5, 6), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.6_Custom"), true),
        };
        _enginePlatformService.Setup(x => x.ScriptFileExtension).Returns("bat");
        _enginePlatformService.Setup(x => x.GetInstalledEngines()).Returns(installedEngines);
        
        const string pluginPath = "C:/dev/Plugins/MyPlugin";
        _filesystem.Directory.CreateDirectory(pluginPath);
        var pluginFile = Path.Join(pluginPath, "MyPlugin.uplugin");
        var descriptor = new PluginDescriptor {
            Version = 1,
            FriendlyName = "My Plugin",
            VersionName = new SemVersion(1, 0, 0),
            Installed = false
        };
        await using (var writer = _filesystem.File.Create(pluginFile)) {
            await JsonSerializer.SerializeAsync(writer, descriptor);
        }

        PluginDescriptor? capturedData = null;
        string? capturedTextFile = null;
        _pluginService.Setup(x => x.SubmitPlugin(It.IsAny<Stream>(),
                It.Is("5.5", EqualityComparer<string>.Default)))
            .Returns(async (Stream x, Version _) => {
                using var archive = new ZipArchive(x);
                var entry = archive.GetEntry("MyPlugin.uplugin")!;
                await using var entryStream = entry.Open();
                capturedData = await JsonSerializer.DeserializeAsync<PluginDescriptor>(entryStream, JsonOptions);
                var subFile = archive.GetEntry(Path.Join("Example", "TextFile.txt"));
                await using var subFileStream = subFile!.Open();
                using var textReader = new StreamReader(subFileStream);
                capturedTextFile = await textReader.ReadToEndAsync();
                return new PluginDetails {
                    Id = 1,
                    Name = "MyPlugin",
                    Versions = []
                };
            });
        
        var batchFilePath = Path.GetFullPath("C:/dev/UnrealEngine/5.5/Engine/Build/BatchFiles/RunUAT.bat");
        _processRunner.Setup(x => x.RunProcess(It.Is<string>(y => y == batchFilePath), 
            It.Is<string[]>(y => y.Length == 3)))
            .Returns(async (string _, string[] args) => {
                var match = PackageRegex().Match(args[2]);
                Assert.That(match.Success);
                var directoryName = match.Groups[1].Value;
                _filesystem.Directory.CreateDirectory(directoryName);
                _filesystem.File.Copy(pluginFile, Path.Join(directoryName, Path.GetFileName(pluginFile)));
                var subDir = Path.Join(directoryName, "Example");
                _filesystem.Directory.CreateDirectory(subDir);
                var textFileName = Path.Join(subDir, "TextFile.txt");
                await _filesystem.File.WriteAllTextAsync(textFileName, "Hello World!");
                return 0;
            });
        
        var engineService = _serviceProvider.GetRequiredService<IEngineService>();
        var result = await engineService.BuildPlugin(_filesystem.FileInfo.New(pluginFile), null);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.EqualTo(0));
            Assert.That(capturedData, Is.Not.Null);
            Assert.That(capturedTextFile, Is.EqualTo("Hello World!"));
        });
        Assert.That(capturedData.Installed, Is.True);
    }

    [Test]
    public async Task TestInstallPlugin() {
        var installedEngines = new List<InstalledEngine> {
            new("5.4", new Version(5, 4), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.4")),
            new("5.5", new Version(5, 5), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.5")),
            new("5.6_Custom", new Version(5, 6), _filesystem.DirectoryInfo.New("C:/dev/UnrealEngine/5.6_Custom"), true),
        };
        _enginePlatformService.Setup(x => x.GetInstalledEngines()).Returns(installedEngines);
        
        const string pluginPath = "C:/dev/Plugins/MyPlugin.zip";
        var dirName = Path.GetDirectoryName(pluginPath);
        Assert.That(dirName, Is.Not.Null);
        _filesystem.Directory.CreateDirectory(dirName);
        await using (var zipFile = _filesystem.File.Create(pluginPath)) {
            using var archive = new ZipArchive(zipFile, ZipArchiveMode.Create);
            var descriptor = new PluginDescriptor {
                Version = 1,
                FriendlyName = "My Plugin",
                VersionName = new SemVersion(1, 0, 0),
                Installed = false
            };
            var pluginEntry = archive.CreateEntry("MyPlugin.uplugin");
            await using (var writer = pluginEntry.Open()) {
                await JsonSerializer.SerializeAsync(writer, descriptor);
            }

            archive.CreateEntry("Example/");
            var textFileName = Path.Join("Example", "TextFile.txt");
            var textFileEntry = archive.CreateEntry(textFileName);
            await using var textFileStream = textFileEntry.Open();
            await using var textWriter = new StreamWriter(textFileStream);
            await textWriter.WriteAsync("Hello World!");
        }

        List<string> targetPlatforms = ["Win64"];
        _pluginService.Setup(x => x.GetPluginFileData("MyPlugin", SemVersionRange.All, "5.4", targetPlatforms))
            .ReturnsAsync((string _, SemVersionRange _, string _) => _filesystem.File.OpenRead(pluginPath));

        var engineService = _serviceProvider.GetRequiredService<IEngineService>();
        var returnCode = await engineService.InstallPlugin("MyPlugin", SemVersionRange.All, "5.4", targetPlatforms);
        Assert.Multiple(() =>
        {
            Assert.That(returnCode, Is.EqualTo(0));
            Assert.That(_filesystem.Directory.Exists(
                Path.Join("C:/dev/UnrealEngine/5.4/Engine/Plugins/Marketplace/.UnrealPluginManager/MyPlugin")),
                Is.True);
        });
    }

    [GeneratedRegex("-Package=\"(.*)\"", RegexOptions.IgnoreCase)]
    private static partial Regex PackageRegex();
}