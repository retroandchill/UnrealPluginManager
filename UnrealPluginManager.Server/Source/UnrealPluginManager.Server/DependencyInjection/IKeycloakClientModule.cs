using Jab;
using UnrealPluginManager.Server.Clients;

namespace UnrealPluginManager.Server.DependencyInjection;

/// <summary>
/// Defines the service module for configuring Keycloak client dependencies
/// within the application, including the HTTP client and associated services.
/// </summary>
[ServiceProviderModule]
[Transient<HttpClient>(Factory = nameof(GetKeycloakAdminHttpClient))]
[Transient<IKeycloakApiKeyClient, KeycloakApiKeyClient>]
public interface IKeycloakClientModule {
  /// Retrieves an instance of an HttpClient configured for communicating with the Keycloak Admin API.
  /// <param name="serviceProvider">
  /// A wrapper around the ServiceProvider used for resolving and accessing the required HttpClient instance.
  /// </param>
  /// <returns>
  /// A configured HttpClient instance for accessing the Keycloak Admin API.
  /// </returns>
  static HttpClient GetKeycloakAdminHttpClient(ServiceProviderWrapper serviceProvider) {
    return serviceProvider.GetRequiredKeyedService<HttpClient>("keycloak_admin_api");
  }
}