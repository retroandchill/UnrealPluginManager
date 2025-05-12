using System.IO.Compression;
using System.Security.Claims;
using System.Security.Principal;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Moq;
using Semver;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Auth.ApiKey;
using UnrealPluginManager.Server.Auth.Policies;
using UnrealPluginManager.Server.Auth.Validators;
using UnrealPluginManager.Server.Database;
using UnrealPluginManager.Server.Database.Users;
using UnrealPluginManager.Server.Tests.Helpers;

namespace UnrealPluginManager.Server.Tests.Auth.Policies;

public class CanSubmitPluginHandlerTest {
  private CloudUnrealPluginManagerContext _dbContext;
  private ServiceProvider _serviceProvider;
  private Mock<IHttpContextAccessor> _httpContextAccessorMock;
  private IJsonService _jsonService;
  private IAuthorizationHandler _handler;

  [SetUp]
  public void Setup() {
    var services = new ServiceCollection();

    // Set up in-memory database
    _dbContext = new TestCloudUnrealPluginManagerContext();
    _dbContext.Database.EnsureCreated();
    services.AddSingleton(_dbContext);

    // Set up mocks
    _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
    services.AddSingleton(_httpContextAccessorMock.Object);

    _jsonService = new JsonService(JsonSerializerOptions.Default);
    services.AddSingleton(_jsonService);

    // Register the real PluginAuthValidator
    services.AddScoped<IPluginAuthValidator, PluginAuthValidator>();

    // Register the handler
    services.AddScoped<IAuthorizationHandler, CanSubmitPluginHandler>();

    _serviceProvider = services.BuildServiceProvider();
    _handler = _serviceProvider.GetRequiredService<IAuthorizationHandler>();
  }

  [TearDown]
  public void TearDown() {
    _dbContext.Database.EnsureDeleted();
    _dbContext.Dispose();
    _serviceProvider.Dispose();
  }

  private IFormFile CreatePluginZipFile(string pluginName) {
    var memoryStream = new MemoryStream();
    using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true)) {
      var upluginEntry = archive.CreateEntry("plugin.json");
      using var writer = new StreamWriter(upluginEntry.Open());
      var manifest = new PluginManifest {
          Name = pluginName,
          Version = new SemVersion(1, 0, 0),
          Source = new SourceLocation {
              Url = new Uri("https://github.com/test/test"),
              Sha = "NotARealSha"
          },
          Dependencies = []
      };
      writer.Write(_jsonService.Serialize(manifest));
    }

    memoryStream.Position = 0;

    var formFile = new Mock<IFormFile>();
    formFile.Setup(f => f.OpenReadStream()).Returns(memoryStream);
    formFile.Setup(f => f.Length).Returns(memoryStream.Length);
    return formFile.Object;
  }

  [Test]
  public async Task HandleRequirementAsync_WhenUserIsPluginOwner_ShouldSucceed() {
    // Arrange
    const string username = "testuser";
    const string pluginName = "TestPlugin";
    var plugin = new Plugin {
        Id = Guid.NewGuid(),
        Name = pluginName
    };

    var user = new User {
        Id = Guid.NewGuid(),
        Username = username,
        Email = "test@example.com",
        Plugins = [plugin]
    };

    _dbContext.Plugins.Add(plugin);
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    // Create form with plugin file
    var formFile = CreatePluginZipFile(pluginName);
    var formCollection = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection {
        formFile
    });

    // Set up HTTP context
    var httpContext = new DefaultHttpContext {
        Request = {
            Form = formCollection,
            ContentType = "multipart/form-data"
        }
    };
    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

    // Set up claims identity
    var identity = new GenericIdentity(username, "TestAuth");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    var authContext = new AuthorizationHandlerContext(
        [new CanSubmitPluginRequirement()],
        claimsPrincipal,
        null);

    // Act
    await _handler.HandleAsync(authContext);

    // Assert
    Assert.That(authContext.HasSucceeded, Is.True);
  }

  [Test]
  public async Task HandleRequirementAsync_WithApiKeyAuthentication_ValidGlobPattern_ShouldSucceed() {
    // Arrange
    const string username = "testuser";
    const string pluginName = "TestPlugin";
    var plugin = new Plugin {
        Id = Guid.NewGuid(),
        Name = pluginName
    };

    var user = new User {
        Id = Guid.NewGuid(),
        Username = username,
        Email = "test@example.com",
        Plugins = [plugin]
    };

    _dbContext.Plugins.Add(plugin);
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    // Create form with plugin file
    var formFile = CreatePluginZipFile(pluginName);
    var formCollection = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection {
        formFile
    });

    // Set up HTTP context
    var httpContext = new DefaultHttpContext {
        Request = {
            Form = formCollection,
            ContentType = "multipart/form-data"
        }
    };
    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

    // Set up claims identity with API key authentication
    var identity = new ClaimsIdentity(ApiKeyClaims.AuthenticationType);
    identity.AddClaim(new Claim(ApiKeyClaims.PluginGlob, "Test*"));
    identity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, username));
    var claimsPrincipal = new ClaimsPrincipal(identity);

    var authContext = new AuthorizationHandlerContext(
        [new CanSubmitPluginRequirement()],
        claimsPrincipal,
        null);

    // Act
    await _handler.HandleAsync(authContext);

    // Assert
    Assert.That(authContext.HasSucceeded, Is.True);
  }

  [Test]
  public async Task HandleRequirementAsync_WithNoFormFile_ShouldNotFail() {
    // Arrange
    var httpContext = new DefaultHttpContext {
        Request = {
            ContentType = "multipart/form-data",
            Form = new FormCollection(new Dictionary<string, StringValues>())
        }
    };
    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

    var identity = new GenericIdentity("testuser", "TestAuth");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    var authContext = new AuthorizationHandlerContext(
        [new CanSubmitPluginRequirement()],
        claimsPrincipal,
        null);

    // Act
    await _handler.HandleAsync(authContext);

    // Assert
    Assert.That(authContext.HasFailed, Is.False);
  }

  [Test]
  public void HandleRequirementAsync_WithInvalidZipFile_ShouldNotFail() {
    // Arrange
    var memoryStream = new MemoryStream([1, 2, 3]); // Invalid ZIP file
    var formFile = new Mock<IFormFile>();
    formFile.Setup(f => f.OpenReadStream()).Returns(memoryStream);
    formFile.Setup(f => f.Length).Returns(memoryStream.Length);

    var formCollection = new FormCollection(new Dictionary<string, StringValues>(), new FormFileCollection {
        formFile.Object
    });

    var httpContext = new DefaultHttpContext {
        Request = {
            ContentType = "multipart/form-data",
            Form = formCollection
        }
    };
    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

    var identity = new GenericIdentity("testuser", "TestAuth");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    var authContext = new AuthorizationHandlerContext(
        [new CanSubmitPluginRequirement()],
        claimsPrincipal,
        null);

    // Act & Assert
    Assert.DoesNotThrowAsync(async () => await _handler.HandleAsync(authContext));
    Assert.That(authContext.HasFailed, Is.False);
  }

  [Test]
  public async Task HandleRequirementAsync_WhenNotAuthenticated_ShouldFail() {
    // Arrange
    var httpContext = new DefaultHttpContext {
        Request = {
            ContentType = "multipart/form-data"
        }
    };
    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

    var identity = new GenericIdentity(""); // No authentication type
    var claimsPrincipal = new ClaimsPrincipal(identity);

    var authContext = new AuthorizationHandlerContext(
        [new CanSubmitPluginRequirement()],
        claimsPrincipal,
        null);

    // Act
    await _handler.HandleAsync(authContext);

    // Assert
    Assert.That(authContext.HasFailed, Is.True);
  }
}