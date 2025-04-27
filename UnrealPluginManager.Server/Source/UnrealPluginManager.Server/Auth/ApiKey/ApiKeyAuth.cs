namespace UnrealPluginManager.Server.Auth.ApiKey;

/// <summary>
/// Provides constants used for API key authentication.
/// </summary>
/// <remarks>
/// This static class contains a constant representing the name of the HTTP header
/// used to supply the API key for authentication.
/// </remarks>
public static class ApiKeyAuth {
  /// <summary>
  /// Specifies the name of the HTTP header used for passing the API key in requests.
  /// </summary>
  /// <remarks>
  /// This constant is utilized in API key authentication mechanisms, ensuring that the
  /// incoming request provides the expected header value for validating access.
  /// </remarks>
  public const string HeaderName = "X-API-Key";
}