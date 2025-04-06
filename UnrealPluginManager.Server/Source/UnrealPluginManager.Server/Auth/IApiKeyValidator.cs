namespace UnrealPluginManager.Server.Auth;

/// <summary>
/// Defines the interface for validating API keys.
/// </summary>
public interface IApiKeyValidator {
  /// <summary>
  /// Validates whether the provided API key is valid.
  /// </summary>
  /// <param name="apiKey">The API key to be validated. Can be null.</param>
  /// <returns>
  /// Returns <c>true</c> if the API key is valid; otherwise, <c>false</c>.
  /// </returns>
  bool IsValid(string? apiKey);
}