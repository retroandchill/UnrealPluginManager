using Jab;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Retro.ReadOnlyParams.Annotations;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.DependencyInjection;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Server.Clients;
using UnrealPluginManager.Server.Database;
using UnrealPluginManager.Server.Services;

namespace UnrealPluginManager.Server.DependencyInjection;

[ServiceProvider]
[Import(typeof(ISystemAbstractionsModule))]
[Singleton(typeof(IHttpContextAccessor), typeof(HttpContextAccessor))]
[Singleton(typeof(IConfiguration), Instance = nameof(_configuration))]
[Singleton(typeof(IJsonService), Factory = nameof(CreateJsonService))]
[Singleton(typeof(IStorageService), typeof(CloudStorageService))]
[Scoped(typeof(UnrealPluginManagerContext), Factory = nameof(GetUnrealPluginManagerContext))]
[Scoped(typeof(CloudUnrealPluginManagerContext))]
[Scoped(typeof(IPluginStructureService), typeof(PluginStructureService))]
[Scoped(typeof(IPluginService), typeof(PluginService))]
[Scoped(typeof(IUserService), typeof(UserService))]
[Transient(typeof(HttpClient), Factory = nameof(GetKeycloakAdminHttpClient))]
[Transient(typeof(IKeycloakApiKeyClient), typeof(KeycloakApiKeyClient))]
public partial class ServerServiceProvider([ReadOnly] IServiceProvider runtimeServiceProvider) {
  private readonly IConfiguration _configuration = runtimeServiceProvider.GetRequiredService<IConfiguration>();

  private IJsonService CreateJsonService() {
    var options = runtimeServiceProvider.GetRequiredService<IOptions<JsonOptions>>();
    return new JsonService(options.Value.JsonSerializerOptions);
  }

  private UnrealPluginManagerContext GetUnrealPluginManagerContext(
      IServiceProvider provider) {
    return provider.GetService<CloudUnrealPluginManagerContext>()
        .RequireNonNull();
  }

  private HttpClient GetKeycloakAdminHttpClient() {
    return runtimeServiceProvider.GetRequiredKeyedService<HttpClient>("keycloak_admin_api");
  }
}