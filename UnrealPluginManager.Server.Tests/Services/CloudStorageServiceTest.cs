using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.IO.Compression;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Config;
using UnrealPluginManager.Server.Services;

namespace UnrealPluginManager.Server.Tests.Services;

public class CloudStorageServiceTest {
    private ServiceProvider _serviceProvider;

    [SetUp]
    public void Setup() {
        var services = new ServiceCollection();

        var mockFilesystem = new MockFileSystem(new Dictionary<string, MockFileData>());
        services.AddSingleton<IFileSystem>(mockFilesystem);

        var mockConfig = new Mock<IConfiguration>();
        services.AddSingleton(mockConfig.Object);
        var mockSection = new Mock<IConfigurationSection>();

        mockConfig.Setup(x => x.GetSection(StorageMetadata.Name)).Returns(mockSection.Object);

        services.AddScoped<IStorageService, CloudStorageService>();
        _serviceProvider = services.BuildServiceProvider();
    }

    [TearDown]
    public void TearDown() {
        _serviceProvider.Dispose();
    }

    [Test]
    public async Task TestStorePlugin() {
        var storageService = _serviceProvider.GetService<IStorageService>()!;

        using var testZip = new MemoryStream();
        using (var zipArchive = new ZipArchive(testZip, ZipArchiveMode.Create, true)) {
            zipArchive.CreateEntry("TestPlugin.uplugin");
        }

        var fileInfo = await storageService.StorePlugin(testZip);
        Assert.Multiple(() => {
            Assert.That(fileInfo.Exists, Is.True);
            Assert.That(fileInfo.Name, Does.StartWith("TestPlugin"));
            Assert.That(fileInfo.Name, Does.EndWith(".zip"));
        });
    }
}