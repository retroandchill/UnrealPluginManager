using Keycloak.AuthServices.Sdk;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Mappers;
using UnrealPluginManager.Core.Model.Users;
using UnrealPluginManager.Server.Clients;

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
[AutoConstructor]
public partial class ApiKeyValidator : IApiKeyValidator {

  private readonly UnrealPluginManagerContext _dbContext;
  private readonly IKeycloakApiKeyClient _keycloakApiKeyClient;

  /// <inheritdoc />
  public async ValueTask<Option<ApiKeyOverview>> LookupApiKey(string? apiKey) {
    if (string.IsNullOrWhiteSpace(apiKey)) {
      return Option<ApiKeyOverview>.None;
    }

    Guid externalId;
    try {
      externalId = await _keycloakApiKeyClient.CheckApiKey("unreal-plugin-manager", apiKey);
    } catch (KeycloakHttpClientException) {
      return Option<ApiKeyOverview>.None;
    }

    return await _dbContext.ApiKeys
        .Where(x => x.ExternalId == externalId)
        .ToApiKeyOverviewQuery()
        .SingleOrDefaultAsync();
  }
}