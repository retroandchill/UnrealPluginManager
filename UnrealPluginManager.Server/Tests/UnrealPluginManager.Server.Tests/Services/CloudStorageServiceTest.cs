using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UnrealPluginManager.Core.Model.Project;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;
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

    services.AddSingleton<IJsonService>(new JsonService(JsonSerializerOptions.Default));
    services.AddSingleton<IStorageService, CloudStorageService>();
    _serviceProvider = services.BuildServiceProvider();
  }

  [TearDown]
  public void TearDown() {
    _serviceProvider.Dispose();
  }

  [Test]
  public void TestSynchronousConfiguration() {
    var storageService = _serviceProvider.GetRequiredService<IStorageService>();
    var baseConfig = storageService.GetConfig<ProjectDescriptor>("TestProject");
    Assert.That(baseConfig.IsNone, Is.True);

    var baseDescriptor = new ProjectDescriptor {
        FileVersion = 3,
        EngineAssociation = "5.5"
    };

    var descriptorOut = storageService.GetConfig("TestProject", baseDescriptor);
    Assert.That(descriptorOut.EngineAssociation, Is.EqualTo("5.5"));

    var otherDescriptor = storageService.GetConfig("OtherProject", () => new ProjectDescriptor {
        FileVersion = 3,
        EngineAssociation = "5.4"
    });
    Assert.That(otherDescriptor.EngineAssociation, Is.EqualTo("5.4"));

    var newDescriptor = new ProjectDescriptor {
        FileVersion = 3,
        EngineAssociation = "5.5",
        Description = "Test description"
    };
    storageService.SaveConfig("TestProject", newDescriptor);

    var descriptorIn = storageService.GetConfig<ProjectDescriptor>("TestProject");
    Assert.That(descriptorIn.IsSome, Is.True);
    Assert.That(descriptorIn.OrElseThrow().Description, Is.EqualTo("Test description"));

    var descriptorOut2 = storageService.GetConfig("TestProject", baseDescriptor);
    Assert.Multiple(() => {
      Assert.That(descriptorOut2.EngineAssociation, Is.EqualTo("5.5"));
      Assert.That(descriptorOut2.Description, Is.EqualTo("Test description"));
    });

    var descriptorOut3 = storageService.GetConfig("TestProject", () => new ProjectDescriptor {
        FileVersion = 3,
        EngineAssociation = "5.4"
    });
    Assert.Multiple(() => {
      Assert.That(descriptorOut3.EngineAssociation, Is.EqualTo("5.5"));
      Assert.That(descriptorOut3.Description, Is.EqualTo("Test description"));
    });
  }


  [Test]
  public async Task TestAsynchronousConfiguration() {
    var storageService = _serviceProvider.GetRequiredService<IStorageService>();
    var baseConfig = await storageService.GetConfigAsync<ProjectDescriptor>("TestProject");
    Assert.That(baseConfig.IsNone, Is.True);

    var baseDescriptor = new ProjectDescriptor {
        FileVersion = 3,
        EngineAssociation = "5.5"
    };

    var descriptorOut = await storageService.GetConfigAsync("TestProject", baseDescriptor);
    Assert.That(descriptorOut.EngineAssociation, Is.EqualTo("5.5"));

    var otherDescriptor = await storageService.GetConfigAsync("OtherProject", () => new ProjectDescriptor {
        FileVersion = 3,
        EngineAssociation = "5.4"
    });
    Assert.That(otherDescriptor.EngineAssociation, Is.EqualTo("5.4"));

    var newDescriptor = new ProjectDescriptor {
        FileVersion = 3,
        EngineAssociation = "5.5",
        Description = "Test description"
    };
    await storageService.SaveConfigAsync("TestProject", newDescriptor);

    var descriptorIn = await storageService.GetConfigAsync<ProjectDescriptor>("TestProject");
    Assert.That(descriptorIn.IsSome, Is.True);
    Assert.That(descriptorIn.OrElseThrow().Description, Is.EqualTo("Test description"));

    var descriptorOut2 = await storageService.GetConfigAsync("TestProject", baseDescriptor);
    Assert.Multiple(() => {
      Assert.That(descriptorOut2.EngineAssociation, Is.EqualTo("5.5"));
      Assert.That(descriptorOut2.Description, Is.EqualTo("Test description"));
    });

    var descriptorOut3 = await storageService.GetConfigAsync("TestProject", () => new ProjectDescriptor {
        FileVersion = 3,
        EngineAssociation = "5.4"
    });
    Assert.Multiple(() => {
      Assert.That(descriptorOut3.EngineAssociation, Is.EqualTo("5.5"));
      Assert.That(descriptorOut3.Description, Is.EqualTo("Test description"));
    });
  }
}