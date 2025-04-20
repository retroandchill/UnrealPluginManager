using System.Text.Encodings.Web;
using UnrealPluginManager.Core.Model.Users;
using Keycloak.AuthServices.Sdk;
using static System.Web.HttpUtility;

namespace UnrealPluginManager.Server.Clients;

/// <summary>
/// A client for interacting with the Keycloak API to manage and validate API keys.
/// </summary>
[AutoConstructor]
public partial class KeycloakApiKeyClient : IKeycloakApiKeyClient {
  private readonly HttpClient _httpClient;

  /// <inheritdoc />
  public async Task<Guid> CheckApiKey(string realm, string apiKey) {
    var path = $"realms/{realm}/api-keys?apiKey={UrlEncode(apiKey)}";
    var response = await _httpClient.GetAsync(path);
    return await response.GetResponseAsync<Guid>();
  }

  /// <inheritdoc />
  public async Task<CreatedApiKey> CreateNewApiKey(string realm, string username, DateTimeOffset expireOn) {
    var expireTime = expireOn.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFK");
    var path = $"realms/{realm}/api-keys?userId={UrlEncode(username)}&expiresOn={UrlEncode(expireTime)}";
    var response = await _httpClient.PostAsync(path, null);
    return (await response.GetResponseAsync<CreatedApiKey>())!;
  }
}