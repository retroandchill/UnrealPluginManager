using Microsoft.AspNetCore.Authorization;

namespace UnrealPluginManager.Server.Auth.Validators;

/// <summary>
/// Defines methods for validating plugin-specific authorization tasks in the system.
/// </summary>
public interface IPluginAuthValidator {
  /// <summary>
  /// Determines whether the current user is authorized to edit a specific plugin.
  /// </summary>
  /// <param name="context">The authorization handler context containing information about the user and their claims.</param>
  /// <param name="pluginName">The name of the plugin to verify authorization for.</param>
  /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the user is authorized to edit the plugin.</returns>
  public Task<bool> CanEditPlugin(AuthorizationHandlerContext context, string pluginName);
}