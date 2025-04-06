namespace UnrealPluginManager.Server.Auth;

/// <summary>
/// Represents the claim types used for API key-based authentication.
/// </summary>
/// <remarks>
/// This class provides constants that are used for defining claims specific to API keys.
/// These claims are typically used in the authentication and authorization processes to
/// verify and manage permissions for different API key holders.
/// </remarks>
public class ApiKeyClaims {

  /// <summary>
  /// Specifies the authentication type used for API key-based authentication.
  /// </summary>
  /// <remarks>
  /// This constant value is utilized to define the authentication mechanism for API key validation.
  /// It is employed to identify the authentication process in operations such as user identity setup
  /// and claims processing within the system.
  /// </remarks>
  public const string AuthenticationType = "ApiKey";

  /// <summary>
  /// Represents the claim type for identifying the global plugin access associated with an API key.
  /// </summary>
  /// <remarks>
  /// This constant is used to specify claims for global plugin access permissions within the API key-based authentication system.
  /// It helps determine the scope of access to plugins for authorization policies and claim evaluation processes.
  /// </remarks>
  public const string PluginGlob = "pluginGlob";

  /// <summary>
  /// Represents a claim type that specifies the list of plugins a user is authorized to interact with.
  /// </summary>
  /// <remarks>
  /// This constant is used in authorization policies to determine whether a user has explicit permissions
  /// to access or interact with specific plugins. It is assigned as a claim in the user's identity
  /// during API key-based authentication processes. The claim value typically stores plugin identifiers
  /// or names that the user is allowed to manage.
  /// </remarks>
  public const string AllowedPlugins = "allowedPlugins";

}