using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Server.Auth.ApiKey;
using UnrealPluginManager.Server.Auth.Policies;
using UnrealPluginManager.Server.Auth.Validators;
using UnrealPluginManager.Server.Database;
using UnrealPluginManager.Server.Database.Users;
using UnrealPluginManager.Server.Tests.Helpers;

namespace UnrealPluginManager.Server.Tests.Auth.Policies;

public class CanEditPluginHandlerTest {
  private CloudUnrealPluginManagerContext _dbContext;
  private ServiceProvider _serviceProvider;
  private Mock<IHttpContextAccessor> _httpContextAccessorMock;
  private IAuthorizationHandler _handler;

  [SetUp]
  public void Setup() {
    var services = new ServiceCollection();

    // Set up in-memory database
    _dbContext = new TestCloudUnrealPluginManagerContext();
    _dbContext.Database.EnsureCreated();
    services.AddSingleton(_dbContext);
    services.AddSingleton<UnrealPluginManagerContext>(_dbContext);

    // Set up mocks
    _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
    services.AddSingleton(_httpContextAccessorMock.Object);

    // Register the real PluginAuthValidator
    services.AddScoped<IPluginAuthValidator, PluginAuthValidator>();

    // Register the handler
    services.AddScoped<IAuthorizationHandler, CanEditPluginHandler>();

    _serviceProvider = services.BuildServiceProvider();
    _handler = _serviceProvider.GetRequiredService<IAuthorizationHandler>();
  }

  [TearDown]
  public void TearDown() {
    _dbContext.Dispose();
    _serviceProvider.Dispose();
  }

  [Test]
  public async Task HandleRequirementAsync_WhenUserIsPluginOwner_ShouldSucceed() {
    // Arrange
    const string username = "testuser";
    var pluginId = Guid.NewGuid();
    var plugin = new Plugin {
        Id = pluginId,
        Name = "TestPlugin"
    };

    var user = new User {
        Id = Guid.NewGuid(),
        Username = username,
        Email = "test@example.com",
        Plugins = [
            new UserPlugin {
                Plugin = plugin
            }
        ]
    };

    _dbContext.Plugins.Add(plugin);
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    // Set up HTTP context
    var httpContext = new DefaultHttpContext {
        Request = {
            ContentType = "application/x-www-form-urlencoded"
        }
    };
    var routeValues = new RouteValueDictionary {
        {
            "pluginId", pluginId.ToString()
        }
    };
    httpContext.Request.RouteValues = routeValues;
    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

    // Set up claims identity
    var identity = new GenericIdentity(username, "TestAuth");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    var authContext = new AuthorizationHandlerContext(
        [
            new CanEditPluginRequirement()
        ],
        claimsPrincipal,
        null);

    // Act
    await _handler.HandleAsync(authContext);

    // Assert
    Assert.That(authContext.HasSucceeded, Is.True);
  }

  [Test]
  public async Task HandleRequirementAsync_WhenUserIsNotPluginOwner_ShouldFail() {
    // Arrange
    const string username = "testuser";
    const string ownerUsername = "owner";
    var pluginId = Guid.NewGuid();
    var plugin = new Plugin {
        Id = pluginId,
        Name = "TestPlugin"
    };

    var owner = new User {
        Id = Guid.NewGuid(),
        Username = ownerUsername,
        Email = "owner@example.com",
        Plugins = [
            new UserPlugin {
                Plugin = plugin
            }
        ]
    };


    _dbContext.Users.Add(owner);
    _dbContext.Users.Add(owner);
    await _dbContext.SaveChangesAsync();

    // Set up HTTP context
    var httpContext = new DefaultHttpContext {
        Request = {
            ContentType = "application/x-www-form-urlencoded"
        }
    };
    var routeValues = new RouteValueDictionary {
        {
            "pluginId", pluginId.ToString()
        }
    };
    httpContext.Request.RouteValues = routeValues;
    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

    // Set up claims identity
    var identity = new GenericIdentity(username, "TestAuth");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    var authContext = new AuthorizationHandlerContext(
        [
            new CanEditPluginRequirement()
        ],
        claimsPrincipal,
        null);

    // Act
    await _handler.HandleAsync(authContext);

    // Assert
    Assert.That(authContext.HasFailed, Is.True);
  }

  [Test]
  public async Task HandleRequirementAsync_WithApiKeyAuthentication_ValidGlobPattern_ShouldSucceed() {
    // Arrange
    const string username = "testuser";
    var pluginId = Guid.NewGuid();
    var plugin = new Plugin {
        Id = pluginId,
        Name = "TestPlugin"
    };

    var user = new User {
        Id = Guid.NewGuid(),
        Username = username,
        Email = "test@example.com",
        Plugins = [
            new UserPlugin {
                Plugin = plugin
            }
        ]
    };

    _dbContext.Plugins.Add(plugin);
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    // Set up HTTP context
    var httpContext = new DefaultHttpContext {
        Request = {
            ContentType = "application/x-www-form-urlencoded"
        }
    };
    var routeValues = new RouteValueDictionary {
        {
            "pluginId", pluginId.ToString()
        }
    };
    httpContext.Request.RouteValues = routeValues;
    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

    // Set up claims identity with API key authentication
    var identity = new ClaimsIdentity(ApiKeyClaims.AuthenticationType);
    identity.AddClaim(new Claim(ApiKeyClaims.PluginGlob, "Test*"));
    identity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, username));
    var claimsPrincipal = new ClaimsPrincipal(identity);

    var authContext = new AuthorizationHandlerContext(
        [
            new CanEditPluginRequirement()
        ],
        claimsPrincipal,
        null);

    // Act
    await _handler.HandleAsync(authContext);

    // Assert
    Assert.That(authContext.HasSucceeded, Is.True);
  }

  [Test]
  public async Task HandleRequirementAsync_WhenPluginNotFound_ShouldSucceed() {
    // Arrange
    const string username = "testuser";
    var nonExistentPluginId = Guid.NewGuid();

    // Set up HTTP context
    var httpContext = new DefaultHttpContext {
        Request = {
            ContentType = "application/x-www-form-urlencoded"
        }
    };
    var routeValues = new RouteValueDictionary {
        {
            "pluginId", nonExistentPluginId.ToString()
        }
    };
    httpContext.Request.RouteValues = routeValues;
    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

    // Set up claims identity
    var identity = new GenericIdentity(username, "TestAuth");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    var authContext = new AuthorizationHandlerContext(
        [
            new CanEditPluginRequirement()
        ],
        claimsPrincipal,
        null);

    // Act
    await _handler.HandleAsync(authContext);

    // Assert
    Assert.That(authContext.HasSucceeded, Is.True);
  }

  [Test]
  public async Task HandleRequirementAsync_WhenHttpContextIsNull_ShouldFail() {
    // Arrange
    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext)null!);

    var identity = new GenericIdentity("testuser", "TestAuth");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    var authContext = new AuthorizationHandlerContext(
        [
            new CanEditPluginRequirement()
        ],
        claimsPrincipal,
        null);

    // Act
    await _handler.HandleAsync(authContext);

    // Assert
    Assert.That(authContext.HasFailed, Is.True);
  }
}