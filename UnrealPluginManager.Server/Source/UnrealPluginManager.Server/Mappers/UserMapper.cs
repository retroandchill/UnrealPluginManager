using Riok.Mapperly.Abstractions;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Users;
using UnrealPluginManager.Server.Database.Users;

namespace UnrealPluginManager.Server.Mappers;

/// <summary>
/// Provides mapping functionality between user-related entities and models.
/// </summary>
[Mapper(UseDeepCloning = true, RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class UserMapper {
  /// <summary>
  /// Maps a <see cref="User"/> object to a <see cref="UserOverview"/> object.
  /// </summary>
  /// <param name="user">The <see cref="User"/> instance containing detailed user information to map from.</param>
  /// <returns>A <see cref="UserOverview"/> object containing the essential overview details derived from the user.</returns>
  public static partial UserOverview ToUserOverview(this User user);

  /// <summary>
  /// Maps an <see cref="ToApiKeyOverview"/> object to an <see cref="ApiKeyOverview"/> object.
  /// </summary>
  /// <param name="user">The <see cref="ToApiKeyOverview"/> instance containing the API key's detailed information to map from.</param>
  /// <returns>An <see cref="ApiKeyOverview"/> object containing the mapped overview of the API key.</returns>
  [MapProperty(nameof(ApiKey.Plugins), nameof(ApiKeyOverview.AllowedPlugins))]
  public static partial ApiKeyOverview ToApiKeyOverview(this ApiKey user);

  /// <summary>
  /// Maps an <see cref="ApiKeyOverview"/> object to an <see cref="ApiKey"/> object.
  /// </summary>
  /// <param name="apiKey">The <see cref="ApiKeyOverview"/> instance providing details about the API key to be mapped.</param>
  /// <param name="externalId">A <see cref="Guid"/> representing the external unique identifier associated with the API key.</param>
  /// <returns>An <see cref="ApiKey"/> object containing detailed information derived from the overview.</returns>
  [MapperIgnoreTarget(nameof(ApiKey.User))]
  [MapperIgnoreTarget(nameof(ApiKey.UserId))]
  [MapperIgnoreTarget(nameof(ApiKey.Plugins))]
  public static partial ApiKey ToApiKey(this ApiKeyOverview apiKey, Guid externalId);

  /// <summary>
  /// Converts a <see cref="Plugin"/> object to a <see cref="PluginIdentifiers"/> object, providing its unique identifier and name.
  /// </summary>
  /// <param name="plugin">The <see cref="Plugin"/> instance to extract identifiers from.</param>
  /// <returns>A <see cref="PluginIdentifiers"/> object containing the unique identifier and name of the plugin.</returns>
  private static PluginIdentifiers GetPluginIdentifiers(Plugin plugin) {
    return new PluginIdentifiers(plugin.Id, plugin.Name);
  }

  /// <summary>
  /// Maps an <see cref="IQueryable{User}"/> to an <see cref="IQueryable{UserOverview}"/> query, enabling the conversion of user entities into their overview representations.
  /// </summary>
  /// <param name="query">The <see cref="IQueryable{User}"/> representing the queryable collection of user entities to map from.</param>
  /// <returns>An <see cref="IQueryable{UserOverview}"/> representing the queryable collection of user overview models.</returns>
  public static partial IQueryable<UserOverview> ToUserOverviewQuery(this IQueryable<User> query);

  /// <summary>
  /// Projects a queryable collection of <see cref="ApiKey"/> entities to a queryable collection of <see cref="ApiKeyOverview"/> models.
  /// </summary>
  /// <param name="query">The <see cref="IQueryable{T}"/> of <see cref="ApiKey"/> to project from.</param>
  /// <returns>An <see cref="IQueryable{T}"/> of <see cref="ApiKeyOverview"/> objects representing the projected data.</returns>
  public static partial IQueryable<ApiKeyOverview> ToApiKeyOverviewQuery(this IQueryable<ApiKey> query);

}