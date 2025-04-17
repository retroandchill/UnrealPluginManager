using LanguageExt;
using UnrealPluginManager.Core.Model.Users;

namespace UnrealPluginManager.Server.Auth;

/// <summary>
/// Defines the interface for validating API keys.
/// </summary>
public interface IApiKeyValidator {
  /// <summary>
  /// Validates and retrieves the overview details of the specified API key, if it exists.
  /// </summary>
  /// <param name="apiKey">The API key to lookup. Can be null or empty.</param>
  /// <returns>
  /// A <see cref="ValueTask"/> encapsulating an optional <see cref="ApiKeyOverview"/> that contains
  /// details about the API key, including its expiration and permissions. If the API key is invalid
  /// or not found, the result will be an empty option.
  /// </returns>
  ValueTask<Option<ApiKeyOverview>> LookupApiKey(string? apiKey);
}