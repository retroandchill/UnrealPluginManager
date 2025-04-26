using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Keycloak.AuthServices.Sdk;
using UnrealPluginManager.Core.Model.Users;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Server.Clients;

/// <summary>
/// A client for interacting with the Keycloak API to manage and validate API keys.
/// </summary>
[AutoConstructor]
public partial class KeycloakApiKeyClient : IKeycloakApiKeyClient {
  private readonly HttpClient _httpClient;
  private readonly IJsonService _jsonService;

  /// <inheritdoc />
  public async Task<Guid> CheckApiKey(string realm, string apiKey) {
    var path = $"realms/{realm}/api-keys";
    var request = new HttpRequestMessage(HttpMethod.Get, path);
    request.Headers.Add("ApiKey", apiKey);

    var response = await _httpClient.SendAsync(request);
    return await response.GetResponseAsync<Guid>();
  }

  /// <inheritdoc />
  public async Task<CreatedApiKey> CreateNewApiKey(string realm, string username, DateTimeOffset expireOn) {
    var expireTime = expireOn.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFK");
    var payload = new {
        UserId = username,
        ExpiresOn = expireTime,
    };

    var content = new StringContent(_jsonService.Serialize(payload), Encoding.UTF8,
        MediaTypeNames.Application.Json);

    var path = $"realms/{realm}/api-keys";
    var response = await _httpClient.PostAsync(path, content);
    return (await response.GetResponseAsync<CreatedApiKey>())
        .RequireNonNull(() => new JsonException("Returned null from Keycloak API."));
  }
}