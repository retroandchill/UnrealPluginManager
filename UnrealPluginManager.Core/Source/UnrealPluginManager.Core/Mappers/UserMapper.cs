using Riok.Mapperly.Abstractions;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Database.Entities.Users;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Users;

namespace UnrealPluginManager.Core.Mappers;

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
  /// Maps an <see cref="ApiKeyDetails"/> object to an <see cref="ApiKeyOverview"/> object.
  /// </summary>
  /// <param name="user">The <see cref="ApiKeyDetails"/> instance containing the API key's detailed information to map from.</param>
  /// <returns>An <see cref="ApiKeyOverview"/> object containing the mapped overview of the API key.</returns>
  public static partial ApiKeyOverview ToApiKeyOverview(this ApiKeyDetails user);

  /// <summary>
  /// Maps an <see cref="ApiKey"/> object to an <see cref="ApiKeyDetails"/> object.
  /// </summary>
  /// <param name="apiKey">The <see cref="ApiKey"/> instance containing the API key's detailed information to map from.</param>
  /// <returns>An <see cref="ApiKeyDetails"/> object containing the detailed representation of the API key.</returns>
  [MapProperty(nameof(ApiKey.Plugins), nameof(ApiKeyOverview.AllowedPlugins))]
  public static partial ApiKeyDetails ToApiKeyDetails(this ApiKey apiKey);

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
  /// Maps an <see cref="IQueryable{ApiKey}"/> to an <see cref="IQueryable{ApiKeyDetails}"/> query, enabling the transformation of entities
  /// into a detailed model object representing API key information.
  /// </summary>
  /// <param name="query">The queryable collection of <see cref="ApiKey"/> instances to be mapped to <see cref="ApiKeyDetails"/>.</param>
  /// <returns>An <see cref="IQueryable{ApiKeyDetails}"/> query representing the mapped data of API keys in detailed model form.</returns>
  public static partial IQueryable<ApiKeyDetails> ToApiKeyDetailsQuery(this IQueryable<ApiKey> query);

}