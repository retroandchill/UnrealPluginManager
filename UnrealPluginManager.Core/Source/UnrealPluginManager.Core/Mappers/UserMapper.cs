using Riok.Mapperly.Abstractions;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Database.Entities.Users;
using UnrealPluginManager.Core.Model.Users;

namespace UnrealPluginManager.Core.Mappers;

[Mapper(UseDeepCloning = true, RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public static partial class UserMapper {

  [MapProperty(nameof(ApiKey.Key), nameof(ApiKeyOverview.HashedKey))]
  [MapProperty(nameof(ApiKey.Plugins), nameof(ApiKeyOverview.AllowedPlugins))]
  public static partial ApiKeyOverview ToApiKeyOverview(this ApiKey apiKey);

  private static Guid GetPluginIds(Plugin plugin) => plugin.Id;

  public static partial IQueryable<ApiKeyOverview> ToApiKeyOverviewQuery(this IQueryable<ApiKey> query);

}