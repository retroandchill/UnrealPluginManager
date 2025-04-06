using LanguageExt;
using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Mappers;
using UnrealPluginManager.Core.Model.Users;

namespace UnrealPluginManager.Server.Auth;

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
  private readonly IPasswordEncoder _passwordEncoder;

  /// <inheritdoc />
  public async ValueTask<Option<ApiKeyOverview>> LookupApiKey(string? apiKey) {
    if (string.IsNullOrWhiteSpace(apiKey)) {
      return Option<ApiKeyOverview>.None;
    }

    var splitKey = apiKey.Split('-');
    if (splitKey.Length != 2) {
      return Option<ApiKeyOverview>.None;
    }

    try {
      var keyId = new Guid(Convert.FromBase64String(splitKey[0]));

      var foundKey = await _dbContext.ApiKeys
          .Where(x => x.Id == keyId)
          .ToApiKeyDetailsQuery()
          .FirstOrDefaultAsync();
      if (foundKey is null) {
        return Option<ApiKeyOverview>.None;
      }

      if (DateTimeOffset.Now > foundKey.ExpiresAt) {
        return Option<ApiKeyOverview>.None;
      }

      var encodedKey = _passwordEncoder.Encode(splitKey[1] + foundKey.Salt);
      return encodedKey == foundKey.PrivateComponent ? foundKey.ToApiKeyOverview() : Option<ApiKeyOverview>.None;
    } catch (ArgumentException) {
      return Option<ApiKeyOverview>.None;
    }
  }
}