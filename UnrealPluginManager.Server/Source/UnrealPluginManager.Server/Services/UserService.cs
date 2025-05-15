using System.Security.Claims;
using Keycloak.AuthServices.Sdk;
using Microsoft.EntityFrameworkCore;
using Retro.ReadOnlyParams.Annotations;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Users;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Server.Clients;
using UnrealPluginManager.Server.Database;
using UnrealPluginManager.Server.Database.Users;
using UnrealPluginManager.Server.Mappers;
using UserMapper = UnrealPluginManager.Server.Mappers.UserMapper;

namespace UnrealPluginManager.Server.Services;

/// <summary>
/// Provides user-related services and operations necessary for the
/// Unreal Plugin Manager application.
/// </summary>
public class UserService(
    [ReadOnly] IHttpContextAccessor httpContextAccessor,
    [ReadOnly] CloudUnrealPluginManagerContext dbContext,
    [ReadOnly] IKeycloakApiKeyClient keycloakApiKeyClient) : IUserService {
  private const string RealmName = "unreal-plugin-manager";

  /// <inheritdoc />
  public async Task<UserOverview> GetActiveUser() {
    var principal = httpContextAccessor.HttpContext?.User;
    principal.RequireNonNull();
    var claimsDict = principal.Claims.ToDictionary(x => x.Type, x => x.Value);
    var username = principal.Identity.RequireNonNull()
        .Name.RequireNonNull();

    var existingUser = await dbContext.Users
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
    dbContext.Users.Add(newUser);
    await dbContext.SaveChangesAsync();

    return UserMapper.ToUserOverview(newUser);
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

    var username = await dbContext.Users
        .Where(x => x.Id == userId)
        .Select(x => x.Username)
        .SingleOrDefaultAsync();
    if (username is null) {
      throw new ContentNotFoundException("User not found.");
    }

    CreatedApiKey createdKey;
    try {
      createdKey = await keycloakApiKeyClient.CreateNewApiKey(RealmName, username, apiKey.ExpiresAt);
    } catch (KeycloakHttpClientException ex) {
      throw new ForeignApiException(ex.StatusCode, "Error creating Api Key", ex);
    }

    var newApiKey = UserMapper.ToApiKey(apiKey, createdKey.Id);
    newApiKey.UserId = userId;
    dbContext.ApiKeys.Add(newApiKey);

    if (apiKey.AllowedPlugins.Count > 0) {
      var pluginIds = apiKey.AllowedPlugins
          .Select(x => x.Id)
          .ToList();
      var existingPluginsCount = await dbContext.Plugins.CountAsync(x => pluginIds.Contains(x.Id));
      if (existingPluginsCount != pluginIds.Count) {
        throw new ContentNotFoundException("One or more allowed plugins not found.");
      }

      dbContext.AllowedPlugins.AddRange(apiKey.AllowedPlugins
                                            .Select(x => new AllowedPlugin {
                                                ApiKeyId = newApiKey.Id,
                                                PluginId = x.Id
                                            }));
    }

    await dbContext.SaveChangesAsync();

    return createdKey.ApiKey;
  }
}