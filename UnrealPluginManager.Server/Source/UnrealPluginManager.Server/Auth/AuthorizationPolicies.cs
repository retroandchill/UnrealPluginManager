namespace UnrealPluginManager.Server.Auth;

/// <summary>
/// Provides a collection of constants representing the authorization policies used within the application.
/// </summary>
/// <remarks>
/// The policies defined in this class are utilized to categorize and enforce access control for various operations.
/// </remarks>
public static class AuthorizationPolicies {
  /// <summary>
  /// Defines an authorization policy specific to contributors who manage plugins within the system.
  /// </summary>
  /// <remarks>
  /// This policy is used to restrict access to operations that involve submitting, adding, or modifying plugin details,
  /// ensuring that only authorized contributors have the necessary permissions to perform such actions.
  /// </remarks>
  public const string PluginContributors = "PluginContributors";
}