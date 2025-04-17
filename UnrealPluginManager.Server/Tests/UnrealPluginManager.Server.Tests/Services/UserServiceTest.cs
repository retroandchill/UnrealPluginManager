using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Users;
using UnrealPluginManager.Core.Tests.Database;
using UnrealPluginManager.Server.Services;

namespace UnrealPluginManager.Server.Tests.Services;

public class UserServiceTest {
  private UnrealPluginManagerContext _context;
  private Mock<IHttpContextAccessor> _httpContextAccessor;
  private ServiceProvider _serviceProvider;
  private IUserService _userService;

  [SetUp]
  public void Setup() {
    var services = new ServiceCollection();
    _context = new TestUnrealPluginManagerContext();
    _context.Database.EnsureCreated();
    services.AddSingleton(_context);

    _httpContextAccessor = new Mock<IHttpContextAccessor>();
    services.AddSingleton(_httpContextAccessor.Object);

    services.AddSingleton<IUserService, UserService>();
    _serviceProvider = services.BuildServiceProvider();
    _userService = _serviceProvider.GetRequiredService<IUserService>();
  }

  [TearDown]
  public void TearDown() {
    _serviceProvider.Dispose();
    _context.Dispose();
  }

  [Test]
  public async Task TestGetActiveUserNotInDatabase() {
    var claimsIdentity = new ClaimsIdentity([
        new Claim(ClaimsIdentity.DefaultNameClaimType, "TestUser"),
        new Claim(ClaimTypes.Email, "Email"),
    ]);
    var user = new ClaimsPrincipal(claimsIdentity);

    var httpContext = new Mock<HttpContext>();
    httpContext.Setup(x => x.User).Returns(user);

    _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

    var activeUser = await _userService.GetActiveUser();
    using (Assert.EnterMultipleScope()) {
      Assert.That(activeUser.Username, Is.EqualTo("TestUser"));
      Assert.That(activeUser.Email, Is.EqualTo("Email"));
    }
  }
  
  [Test]
  public async Task TestGetActiveUserAlreadyInDatabase() {
    var claimsIdentity = new ClaimsIdentity([
        new Claim(ClaimsIdentity.DefaultNameClaimType, "TestUser"),
        new Claim(ClaimTypes.Email, "Email"),
    ]);
    var user = new ClaimsPrincipal(claimsIdentity);

    var httpContext = new Mock<HttpContext>();
    httpContext.Setup(x => x.User).Returns(user);

    var existingUser = new User {
        Username = "TestUser",
        Email = "Email"
    };
    _context.Users.Add(existingUser);
    await _context.SaveChangesAsync();

    _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

    var activeUser = await _userService.GetActiveUser();
    using (Assert.EnterMultipleScope()) {
      Assert.That(activeUser.Id, Is.EqualTo(existingUser.Id));
      Assert.That(activeUser.Username, Is.EqualTo("TestUser"));
      Assert.That(activeUser.Email, Is.EqualTo("Email"));
    }
  }
}