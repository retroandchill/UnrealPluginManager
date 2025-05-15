using Keycloak.AuthServices.Sdk;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using Retro.ReadOnlyParams.Annotations;
using UnrealPluginManager.Core.Model.Users;
using UnrealPluginManager.Server.Clients;
using UnrealPluginManager.Server.Database;
using UnrealPluginManager.Server.Mappers;

namespace UnrealPluginManager.Server.Auth.ApiKey;

/// <summary>
/// Provides functionality to validate API keys for authorization purposes.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IApiKeyValidator"/> interface and defines
/// the logic to determine whether a given API key is valid. It is typically used
/// in the context of server authentication mechanisms and is registered as a service
/// for dependency injection.
/// </remarks>
public class ApiKeyValidator(
    [ReadOnly] CloudUnrealPluginManagerContext dbContext,
    [ReadOnly] IKeycloakApiKeyClient keycloakApiKeyClient) : IApiKeyValidator {
  /// <inheritdoc />
  public async ValueTask<Option<ApiKeyOverview>> LookupApiKey(string? apiKey) {
    if (string.IsNullOrWhiteSpace(apiKey)) {
      return Option<ApiKeyOverview>.None;
    }

    Guid externalId;
    try {
      externalId = await keycloakApiKeyClient.CheckApiKey("unreal-plugin-manager", apiKey);
    } catch (KeycloakHttpClientException) {
      return Option<ApiKeyOverview>.None;
    }

    return await dbContext.ApiKeys
        .Where(x => x.ExternalId == externalId)
        .ToApiKeyOverviewQuery()
        .SingleOrDefaultAsync();
  }
}