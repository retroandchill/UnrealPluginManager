using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace UnrealPluginManager.Server.Auth;

/// <summary>
/// Provides constants for authentication schemes used in the application.
/// </summary>
public static class AuthenticationSchemes {
  /// <summary>
  /// Represents the constant identifier for the API Key authentication scheme.
  /// </summary>
  public const string ApiKey = "ApiKey";

  /// <summary>
  /// Represents a combined constant identifier for the Bearer and API Key authentication schemes.
  /// </summary>
  public const string BearerOrApiKey = $"{JwtBearerDefaults.AuthenticationScheme},{ApiKey}";
  
}