﻿using System.Security.Claims;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using LanguageExt.UnsafeValueAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Database.Entities.Users;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Users;
using UnrealPluginManager.Core.Tests.Database;
using UnrealPluginManager.Server.Auth;
using UnrealPluginManager.Server.Auth.ApiKey;
using UnrealPluginManager.Server.Services;

namespace UnrealPluginManager.Server.Tests.Services;

public class UserServiceTest {
  private UnrealPluginManagerContext _context;
  private Mock<IHttpContextAccessor> _httpContextAccessor;
  private ServiceProvider _serviceProvider;
  private IUserService _userService;
  private IApiKeyValidator _apiKeyValidator;

  [SetUp]
  public void Setup() {
    var services = new ServiceCollection();
    _context = new TestUnrealPluginManagerContext();
    _context.Database.EnsureCreated();
    services.AddSingleton(_context);

    _httpContextAccessor = new Mock<IHttpContextAccessor>();
    services.AddSingleton(_httpContextAccessor.Object);

    services.AddSingleton<IUserService, UserService>()
        .AddSingleton<HashAlgorithm>(_ => SHA512.Create())
        .AddSingleton<IPasswordEncoder, PasswordEncoder>()
        .AddSingleton<IApiKeyValidator, ApiKeyValidator>();

    _serviceProvider = services.BuildServiceProvider();
    _userService = _serviceProvider.GetRequiredService<IUserService>();
    _apiKeyValidator = _serviceProvider.GetRequiredService<IApiKeyValidator>();
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
        new Claim(ClaimTypes.Email, "email@gmail.com"),
    ]);
    var user = new ClaimsPrincipal(claimsIdentity);

    var httpContext = new Mock<HttpContext>();
    httpContext.Setup(x => x.User).Returns(user);

    _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

    var activeUser = await _userService.GetActiveUser();
    using (Assert.EnterMultipleScope()) {
      Assert.That(activeUser.Username, Is.EqualTo("TestUser"));
      Assert.That(activeUser.Email, Is.EqualTo("email@gmail.com"));
    }
  }

  [Test]
  public async Task TestGetActiveUserAlreadyInDatabase() {
    var claimsIdentity = new ClaimsIdentity([
        new Claim(ClaimsIdentity.DefaultNameClaimType, "TestUser"),
        new Claim(ClaimTypes.Email, "email@gmail.com"),
    ]);
    var user = new ClaimsPrincipal(claimsIdentity);

    var httpContext = new Mock<HttpContext>();
    httpContext.Setup(x => x.User).Returns(user);

    var existingUser = new User {
        Username = "TestUser",
        Email = "email@gmail.com"
    };
    _context.Users.Add(existingUser);
    await _context.SaveChangesAsync();

    _httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext.Object);

    var activeUser = await _userService.GetActiveUser();
    using (Assert.EnterMultipleScope()) {
      Assert.That(activeUser.Id, Is.EqualTo(existingUser.Id));
      Assert.That(activeUser.Username, Is.EqualTo("TestUser"));
      Assert.That(activeUser.Email, Is.EqualTo("email@gmail.com"));
    }
  }

  [Test]
  public async Task TestCreateApiKey() {
    var user = new User {
        Username = "TestUser",
        Email = "email@gmail.com"
    };
    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    var newApiKey = await _userService.CreateApiKey(user.Id, new ApiKeyOverview {
        DisplayName = "TestKey",
        ExpiresAt = DateTimeOffset.Now.AddDays(1),
        PluginGlob = "*"
    });

    var foundApiKey = await _apiKeyValidator.LookupApiKey(newApiKey);
    Assert.That(foundApiKey.IsSome, Is.True);

    var apiKeyValue = foundApiKey.ValueUnsafe();
    Assert.That(apiKeyValue, Is.Not.Null);
    Assert.That(apiKeyValue.DisplayName, Is.EqualTo("TestKey"));
  }

  [Test]
  public async Task TestCreateApiKeyInvalidParameters() {
    var user = new User {
        Username = "TestUser",
        Email = "email@gmail.com"
    };
    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    var apiKey = new ApiKeyOverview {
        DisplayName = "TestKey",
        ExpiresAt = DateTimeOffset.Now.Subtract(TimeSpan.FromDays(1)),
        PluginGlob = "*"
    };
    Assert.ThrowsAsync<BadArgumentException>(() => _userService.CreateApiKey(user.Id, apiKey));

    apiKey.ExpiresAt = DateTimeOffset.Now.AddDays(1);
    apiKey.PluginGlob = null;
    Assert.ThrowsAsync<BadArgumentException>(() => _userService.CreateApiKey(user.Id, apiKey));

    apiKey.PluginGlob = "*";
    apiKey.AllowedPlugins = [new PluginIdentifiers(Guid.NewGuid(), "Plugin1")];
    Assert.ThrowsAsync<BadArgumentException>(() => _userService.CreateApiKey(user.Id, apiKey));

    apiKey.AllowedPlugins = [];
    Assert.ThrowsAsync<ContentNotFoundException>(() => _userService.CreateApiKey(Guid.CreateVersion7(), apiKey));
  }

  [Test]
  public async Task TestCreateApiKeyWithPlugins() {
    var user = new User {
        Username = "TestUser",
        Email = "email@gmail.com"
    };
    _context.Users.Add(user);

    var plugin = new Plugin {
        Name = "Plugin1",
    };
    _context.Plugins.Add(plugin);
    await _context.SaveChangesAsync();

    var newApiKey = await _userService.CreateApiKey(user.Id, new ApiKeyOverview {
        DisplayName = "TestKey",
        ExpiresAt = DateTimeOffset.Now.AddDays(1),
        AllowedPlugins = [
            new PluginIdentifiers(plugin.Id, plugin.Name)
        ]
    });

    var foundApiKey = await _apiKeyValidator.LookupApiKey(newApiKey);
    Assert.That(foundApiKey.IsSome, Is.True);

    var apiKeyValue = foundApiKey.ValueUnsafe();
    Assert.That(apiKeyValue, Is.Not.Null);
    using (Assert.EnterMultipleScope()) {
      Assert.That(apiKeyValue.DisplayName, Is.EqualTo("TestKey"));
      Assert.That(apiKeyValue.AllowedPlugins, Has.Count.EqualTo(1));
    }
  }
  
  [Test]
  public async Task TestCreateApiKeyWithInvalidPlugins() {
    var user = new User {
        Username = "TestUser",
        Email = "email@gmail.com"
    };
    _context.Users.Add(user);

    var plugin = new Plugin {
        Name = "Plugin1",
    };
    _context.Plugins.Add(plugin);
    await _context.SaveChangesAsync();

    var apiKey = new ApiKeyOverview {
        DisplayName = "TestKey",
        ExpiresAt = DateTimeOffset.Now.AddDays(1),
        AllowedPlugins = [
            new PluginIdentifiers(plugin.Id, plugin.Name),
            new PluginIdentifiers(plugin.Id, plugin.Name)
        ]
    };
    Assert.ThrowsAsync<BadArgumentException>(() => _userService.CreateApiKey(user.Id, apiKey));
    
    apiKey.AllowedPlugins = [
        new PluginIdentifiers(Guid.NewGuid(), "Plugin1"),
        new PluginIdentifiers(Guid.NewGuid(), "Plugin2")
    ];
    Assert.ThrowsAsync<ContentNotFoundException>(() => _userService.CreateApiKey(user.Id, apiKey));
  }
}