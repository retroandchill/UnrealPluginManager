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

  private static PluginIdentifiers GetPluginIdentifiers(Plugin plugin) {
    return new PluginIdentifiers(plugin.Id, plugin.Name);
  }

  /// <summary>
  /// Maps an <see cref="IQueryable{ApiKey}"/> to an <see cref="IQueryable{ApiKeyDetails}"/> query, enabling the transformation of entities
  /// into a detailed model object representing API key information.
  /// </summary>
  /// <param name="query">The queryable collection of <see cref="ApiKey"/> instances to be mapped to <see cref="ApiKeyDetails"/>.</param>
  /// <returns>An <see cref="IQueryable{ApiKeyDetails}"/> query representing the mapped data of API keys in detailed model form.</returns>
  public static partial IQueryable<ApiKeyDetails> ToApiKeyDetailsQuery(this IQueryable<ApiKey> query);

}