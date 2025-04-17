using System.Security.Claims;
using System.Security.Cryptography;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Users;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Mappers;
using UnrealPluginManager.Core.Model.Users;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Server.Auth;

namespace UnrealPluginManager.Server.Services;

/// <summary>
/// Provides user-related services and operations necessary for the
/// Unreal Plugin Manager application.
/// </summary>
[AutoConstructor]
public partial class UserService : IUserService {
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly UnrealPluginManagerContext _dataContext;
  private readonly IPasswordEncoder _passwordEncoder;

  /// <inheritdoc />
  public async Task<UserOverview> GetActiveUser() {
    var principal = _httpContextAccessor.HttpContext?.User;
    principal.RequireNonNull();
    var claimsDict = principal.Claims.ToDictionary(x => x.Type, x => x.Value);
    var username = principal.Identity.RequireNonNull()
        .Name.RequireNonNull();
    
    var existingUser = await _dataContext.Users
        .Where(x => x.Username == username)
        .ToUserOverviewQuery()
        .FirstOrDefaultAsync();
    if (existingUser is not null) {
      return existingUser;
    }

    var newUser = new User {
        Username = username,
        Email = claimsDict[ClaimTypes.Email]
    }; 
    _dataContext.Users.Add(newUser);
    await _dataContext.SaveChangesAsync();
    
    return newUser.ToUserOverview();
  }
  
  /// <inheritdoc />
  public async Task<string> CreateApiKey(Guid userId, ApiKeyOverview apiKey) {
    var now = DateTimeOffset.Now;
    if (apiKey.ExpiresAt < now) {
      throw new BadArgumentException("Api key cannot expire in the past.");
    }

    if (apiKey.ExpiresAt > now.AddYears(1)) {
      throw new BadArgumentException("Api key cannot expire in more than one year.");
    }

    if (apiKey.PluginGlob is not null && apiKey.AllowedPlugins.Count > 0) {
      throw new BadArgumentException("Api key cannot specify both a plugin glob and allowed plugins.");
    }

    if (apiKey.PluginGlob is null && apiKey.AllowedPlugins.Count == 0) {
      throw new BadArgumentException("Api key must specify either a plugin glob or allowed plugins.");
    }

    if (apiKey.AllowedPlugins.Count > 0 &&
        apiKey.AllowedPlugins.Count > apiKey.AllowedPlugins.DistinctBy(x => x.Id).Count()) {
      throw new BadArgumentException("Api key cannot specify duplicate allowed plugins.");
    }
    
    if (!await _dataContext.Users.AnyAsync(x => x.Id == userId)) {
      throw new ContentNotFoundException("User not found.");
    }
    
    var rng = RandomNumberGenerator.Create();
    var privateComponentBytes = new byte[16];
    rng.GetBytes(privateComponentBytes);
    var privateComponent = Convert.ToBase64String(privateComponentBytes);
    
    var (encodedPrivateComponent, salt) = _passwordEncoder.EncodeAndSalt(privateComponent);
    var newApiKey = apiKey.ToApiKey(encodedPrivateComponent, salt);
    newApiKey.UserId = userId;
    _dataContext.ApiKeys.Add(newApiKey);

    if (apiKey.AllowedPlugins.Count > 0) {
      var pluginIds = apiKey.AllowedPlugins
          .Select(x => x.Id)
          .ToList();
      var existingPluginsCount = await _dataContext.Plugins.CountAsync(x => pluginIds.Contains(x.Id));
      if (existingPluginsCount != pluginIds.Count) {
        throw new ContentNotFoundException("One or more allowed plugins not found.");
      }
      
      _dataContext.AllowedPlugins.AddRange(apiKey.AllowedPlugins
                                               .Select(x => new AllowedPlugin {
                                                   ApiKeyId = newApiKey.Id,
                                                   PluginId = x.Id
                                               }));
    }

    await _dataContext.SaveChangesAsync();

    var publicComponent = Convert.ToBase64String(newApiKey.Id.ToByteArray());
    return $"{publicComponent}-{privateComponent}";
  }
}