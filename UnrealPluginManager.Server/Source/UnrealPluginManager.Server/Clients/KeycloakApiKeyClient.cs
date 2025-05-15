using System.Net.Mime;
using System.Text;
using System.Text.Json;
using Keycloak.AuthServices.Sdk;
using Retro.ReadOnlyParams.Annotations;
using UnrealPluginManager.Core.Model.Users;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Server.Clients;

/// <summary>
/// A client for interacting with the Keycloak API to manage and validate API keys.
/// </summary>
public class KeycloakApiKeyClient(
    [ReadOnly] HttpClient httpClient,
    [ReadOnly] IJsonService jsonService) : IKeycloakApiKeyClient {
  /// <inheritdoc />
  public async Task<Guid> CheckApiKey(string realm, string apiKey) {
    var path = $"realms/{realm}/api-keys";
    var request = new HttpRequestMessage(HttpMethod.Get, path);
    request.Headers.Add("ApiKey", apiKey);

    var response = await httpClient.SendAsync(request);
    return await response.GetResponseAsync<Guid>();
  }

  /// <inheritdoc />
  public async Task<CreatedApiKey> CreateNewApiKey(string realm, string username, DateTimeOffset expireOn) {
    var expireTime = expireOn.ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFK");
    var payload = new {
        Username = username,
        ExpiresOn = expireTime,
    };

    var content = new StringContent(jsonService.Serialize(payload), Encoding.UTF8,
                                    MediaTypeNames.Application.Json);

    var path = $"realms/{realm}/api-keys";
    var response = await httpClient.PostAsync(path, content);
    return (await response.GetResponseAsync<CreatedApiKey>())
        .RequireNonNull(() => new JsonException("Returned null from Keycloak API."));
  }
}