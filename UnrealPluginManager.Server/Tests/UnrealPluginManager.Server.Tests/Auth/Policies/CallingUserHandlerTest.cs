using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UnrealPluginManager.Server.Auth.Policies;
using UnrealPluginManager.Server.Database;
using UnrealPluginManager.Server.Database.Users;
using UnrealPluginManager.Server.Tests.Helpers;

namespace UnrealPluginManager.Server.Tests.Auth.Policies;

public class CallingUserHandlerTest {
  private CloudUnrealPluginManagerContext _dbContext;
  private ServiceProvider _serviceProvider;
  private Mock<IHttpContextAccessor> _httpContextAccessorMock;
  private IAuthorizationHandler _handler;

  [SetUp]
  public void Setup() {
    var services = new ServiceCollection();

    _dbContext = new TestCloudUnrealPluginManagerContext();
    _dbContext.Database.EnsureCreated();
    services.AddSingleton(_dbContext);

    // Set up mocks
    _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
    services.AddSingleton(_httpContextAccessorMock.Object);

    // Create the handler
    services.AddScoped<IAuthorizationHandler, CallingUserHandler>();
    _serviceProvider = services.BuildServiceProvider();
    _handler = _serviceProvider.GetRequiredService<IAuthorizationHandler>();
  }

  [TearDown]
  public void TearDown() {
    _dbContext.Dispose();
    _serviceProvider.Dispose();
  }

  [Test]
  public async Task HandleRequirementAsync_WhenUserMatchesRouteId_ShouldSucceed() {
    // Arrange
    var userId = Guid.CreateVersion7();
    const string username = "testuser";

    // Add test user to database
    var user = new User {
        Id = userId,
        Username = username,
        Email = "email@test.com"
    };
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    // Set up HTTP context with route values
    var httpContext = new DefaultHttpContext();
    var routeValues = new RouteValueDictionary {
        {
            "userId", userId.ToString()
        }
    };
    httpContext.Request.RouteValues = routeValues;
    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

    // Set up claims identity
    var identity = new GenericIdentity(username, "TestAuth");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    // Create authorization context
    var authContext = new AuthorizationHandlerContext(
        [
            new CallingUserRequirement()
        ],
        claimsPrincipal,
        null);

    // Act
    await _handler.HandleAsync(authContext);

    // Assert
    Assert.That(authContext.HasSucceeded, Is.True);
  }

  [Test]
  public async Task HandleRequirementAsync_WhenUserDoesNotMatchRouteId_ShouldFail() {
    // Arrange
    var userId = Guid.NewGuid();
    var differentUserId = Guid.NewGuid();
    const string username = "testuser";

    // Add test user to database
    var user = new User {
        Id = userId,
        Username = username,
        Email = "email@test.com"
    };
    _dbContext.Users.Add(user);
    await _dbContext.SaveChangesAsync();

    // Set up HTTP context with different user ID in route
    var httpContext = new DefaultHttpContext();
    var routeValues = new RouteValueDictionary {
        {
            "userId", differentUserId.ToString()
        }
    };
    httpContext.Request.RouteValues = routeValues;
    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

    // Set up claims identity
    var identity = new GenericIdentity(username, "TestAuth");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    // Create authorization context
    var authContext = new AuthorizationHandlerContext(
        [
            new CallingUserRequirement()
        ],
        claimsPrincipal,
        null);

    // Act
    await _handler.HandleAsync(authContext);

    // Assert
    Assert.That(authContext.HasFailed, Is.True);
  }

  [Test]
  public async Task HandleRequirementAsync_WhenHttpContextIsNull_ShouldFail() {
    // Arrange
    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns((HttpContext) null!);

    var identity = new GenericIdentity("testuser", "TestAuth");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    var authContext = new AuthorizationHandlerContext(
        [
            new CallingUserRequirement()
        ],
        claimsPrincipal,
        null);

    // Act
    await _handler.HandleAsync(authContext);

    // Assert
    Assert.That(authContext.HasFailed, Is.True);
  }

  [Test]
  public async Task HandleRequirementAsync_WhenUserIsNotAuthenticated_ShouldFail() {
    // Arrange
    var httpContext = new DefaultHttpContext();
    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

    var identity = new GenericIdentity("testuser"); // No authentication type = not authenticated
    var claimsPrincipal = new ClaimsPrincipal(identity);

    var authContext = new AuthorizationHandlerContext(
        [
            new CallingUserRequirement()
        ],
        claimsPrincipal,
        null);

    // Act
    await _handler.HandleAsync(authContext);

    // Assert
    Assert.That(authContext.HasFailed, Is.True);
  }

  [Test]
  public async Task HandleRequirementAsync_WhenRouteValueIsMissing_ShouldFail() {
    // Arrange
    var httpContext = new DefaultHttpContext {
        Request = {
            RouteValues = new RouteValueDictionary() // Empty route values
        }
    };
    _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

    var identity = new GenericIdentity("testuser", "TestAuth");
    var claimsPrincipal = new ClaimsPrincipal(identity);

    var authContext = new AuthorizationHandlerContext(
        [
            new CallingUserRequirement()
        ],
        claimsPrincipal,
        null);

    // Act
    await _handler.HandleAsync(authContext);

    // Assert
    Assert.That(authContext.HasFailed, Is.True);
  }

}