using Jab;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Retro.ReadOnlyParams.Annotations;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.DependencyInjection;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Auth;
using UnrealPluginManager.Server.Auth.ApiKey;
using UnrealPluginManager.Server.Auth.Policies;
using UnrealPluginManager.Server.Auth.Validators;
using UnrealPluginManager.Server.Clients;
using UnrealPluginManager.Server.Database;
using UnrealPluginManager.Server.Exceptions;
using UnrealPluginManager.Server.Services;

namespace UnrealPluginManager.Server.DependencyInjection;

/// <summary>
/// The ServerServiceProvider class is a service provider implementation that manages dependency injection
/// and service resolution for the Unreal Plugin Manager server application. It is responsible for
/// configuring and providing the necessary services and dependencies required by the application.
/// This class uses attributes to specify injection behaviors and service lifetimes
/// such as Singleton, Scoped, and Transient. Furthermore, it integrates with different interfaces
/// and factory methods to set up various services and modules.
/// <remarks>
/// The class integrates custom configuration methods and factory logic for specific services
/// such as JSON handling, storage, authorization, plugin management, and HTTP client services for Keycloak.
/// </remarks>
/// </summary>
[ServiceProvider]
[Import(typeof(ISystemAbstractionsModule))]
[Singleton(typeof(IHttpContextAccessor), typeof(HttpContextAccessor))]
[Singleton(typeof(IConfiguration), Factory = nameof(GetConfiguration))]
[Singleton(typeof(IJsonService), Factory = nameof(CreateJsonService))]
[Singleton(typeof(IStorageService), typeof(CloudStorageService))]
[Singleton(typeof(IExceptionHandler), typeof(ServerExceptionHandler))]
[Singleton(typeof(IHostEnvironment), Factory = nameof(GetHostEnvironment))]
[Singleton(typeof(ILogger<>), Factory = nameof(GetExceptionHandlerLogger))]
[Scoped(typeof(UnrealPluginManagerContext), Factory = nameof(GetUnrealPluginManagerContext))]
[Scoped(typeof(CloudUnrealPluginManagerContext))]
[Scoped(typeof(IPluginStructureService), typeof(PluginStructureService))]
[Scoped(typeof(IPluginService), typeof(PluginService))]
[Scoped(typeof(IUserService), typeof(UserService))]
[Scoped(typeof(IApiKeyValidator), typeof(ApiKeyValidator))]
[Scoped(typeof(IPluginAuthValidator), typeof(PluginAuthValidator))]
[Scoped(typeof(IAuthorizationHandler), typeof(CanSubmitPluginHandler))]
[Scoped(typeof(IAuthorizationHandler), typeof(CanEditPluginHandler))]
[Scoped(typeof(IAuthorizationHandler), typeof(CallingUserHandler))]
[Transient(typeof(HttpClient), Factory = nameof(GetKeycloakAdminHttpClient))]
[Transient(typeof(IKeycloakApiKeyClient), typeof(KeycloakApiKeyClient))]
[JabCopyExclude(typeof(IConfiguration), typeof(IHostEnvironment), typeof(HttpClient), typeof(ILogger<>))]
public partial class ServerServiceProvider([ReadOnly] IServiceProvider runtimeServiceProvider) {
  private IConfiguration GetConfiguration() {
    return runtimeServiceProvider.GetRequiredService<IConfiguration>();
  }

  private IHostEnvironment GetHostEnvironment() {
    return runtimeServiceProvider.GetRequiredService<IHostEnvironment>();
  }

  private ILogger<T> GetExceptionHandlerLogger<T>() {
    return runtimeServiceProvider.GetRequiredService<ILogger<T>>();
  }

  private JsonService CreateJsonService() {
    var options = runtimeServiceProvider.GetRequiredService<IOptions<JsonOptions>>();
    return new JsonService(options.Value.JsonSerializerOptions);
  }

  private static CloudUnrealPluginManagerContext GetUnrealPluginManagerContext(
      CloudUnrealPluginManagerContext unrealPluginManagerContext) {
    return unrealPluginManagerContext;
  }

  private HttpClient GetKeycloakAdminHttpClient() {
    return runtimeServiceProvider.GetRequiredKeyedService<HttpClient>("keycloak_admin_api");
  }
}