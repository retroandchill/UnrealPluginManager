namespace UnrealPluginManager.Server.Auth;

/// <summary>
/// Provides a collection of constants representing the authorization policies used within the application.
/// </summary>
/// <remarks>
/// The policies defined in this class are utilized to categorize and enforce access control for various operations.
/// </remarks>
public static class AuthorizationPolicies {
  /// <summary>
  /// Represents an authorization policy permitting users to submit plugins for processing.
  /// </summary>
  /// <remarks>
  /// This policy is utilized to enforce access control for operations that involve the submission of plugins.
  /// Endpoints such as plugins/submit require this policy to ensure only authorized users can perform plugin submissions.
  /// </remarks>
  public const string CanSubmitPlugin = "CanSubmitPlugin";

  /// <summary>
  /// Represents an authorization policy allowing users to modify or edit existing plugins.
  /// </summary>
  /// <remarks>
  /// This policy is applied to enforce access control for operations involving the editing of plugins.
  /// Endpoints related to plugin modification require this policy to ensure only authorized users can make changes to plugin data.
  /// </remarks>
  public const string CanEditPlugin = "CanEditPlugin";

  /// <summary>
  /// Represents an authorization policy that validates and identifies the currently authenticated user.
  /// </summary>
  /// <remarks>
  /// This policy ensures that API endpoints requiring user-specific authorization can verify and operate with the context of the calling user.
  /// It is commonly applied to secure actions such as creating API keys or performing user-specific operations.
  /// </remarks>
  public const string CallingUser = "CallingUser";
}