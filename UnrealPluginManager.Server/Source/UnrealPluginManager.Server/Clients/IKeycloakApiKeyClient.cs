using UnrealPluginManager.Core.Model.Users;

namespace UnrealPluginManager.Server.Clients;

/// <summary>
/// Represents a client interface for interacting with the Keycloak API to manage API keys.
/// </summary>
public interface IKeycloakApiKeyClient {
  /// <summary>
  /// Validates and retrieves the unique identifier for an API key within a specified realm.
  /// </summary>
  /// <param name="realm">The name of the Keycloak realm where the API key is being checked.</param>
  /// <param name="apiKey">The API key to validate.</param>
  /// <returns>A <see cref="Guid"/> representing the unique identifier of the API key if valid.</returns>
  Task<Guid> CheckApiKey(string realm, string apiKey);

  /// <summary>
  /// Creates a new API key for a specified user within a given Keycloak realm and sets its expiration time.
  /// </summary>
  /// <param name="realm">The name of the Keycloak realm where the API key is being created.</param>
  /// <param name="username">The username for whom the API key is being created.</param>
  /// <param name="expireOn">The date and time when the API key should expire.</param>
  /// <returns>A <see cref="CreatedApiKey"/> instance containing the API key details, including its ID, value, and expiration timestamp.</returns>
  Task<CreatedApiKey> CreateNewApiKey(string realm, string username, DateTimeOffset expireOn);

}