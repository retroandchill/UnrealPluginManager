using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Server.Auth.ApiKey;

namespace UnrealPluginManager.Server.Auth.Validators;

/// <summary>
/// Validates user authorization for plugin-related operations.
/// </summary>
/// <remarks>
/// This class provides a mechanism to determine if a user has the necessary permissions
/// to edit a specific plugin. It processes user claims and performs database lookups to
/// confirm authorization.
/// </remarks>
[AutoConstructor]
public partial class PluginAuthValidator : IPluginAuthValidator {
  private readonly UnrealPluginManagerContext _dbContext;

  /// <summary>
  /// Determines whether the current user has the necessary permissions to edit the specified plugin.
  /// </summary>
  /// <param name="context">
  /// The <see cref="AuthorizationHandlerContext"/> containing the user's authentication details and claims.
  /// </param>
  /// <param name="pluginName">
  /// The name of the plugin for which the edit permissions need to be verified.
  /// </param>
  /// <returns>
  /// A task that represents the asynchronous operation. The task result contains a boolean value indicating
  /// whether the user is authorized to edit the specified plugin.
  /// </returns>
  public async Task<bool> CanEditPlugin(AuthorizationHandlerContext context, string pluginName) {
    ArgumentNullException.ThrowIfNull(context.User.Identity);
    if (context.User.Identity.AuthenticationType == ApiKeyClaims.AuthenticationType) {
      if (context.User.HasClaim(c => c.Type == ApiKeyClaims.PluginGlob && pluginName.Like(c.Value)
                                     || c.Type == ApiKeyClaims.AllowedPlugins && c.Value == pluginName)) {
        return await _dbContext.Plugins
            .Where(x => x.Name == pluginName)
            .SelectMany(x => x.Owners)
            .AnyAsync(x => x.Username == context.User.Identity.Name);
      }
    } else {
      ArgumentNullException.ThrowIfNull(context.User.Identity.Name);
      var validPlugin = await _dbContext.Plugins
          .Include(x => x.Owners
                       .Where(y => y.Username == context.User.Identity.Name))
          .Where(x => x.Name == pluginName)
          .Select(x => new {
              x.Name,
              Owners = x.Owners.Select(y => y.Username).ToList()
          })
          .FirstOrDefaultAsync();
      if (validPlugin is null || !validPlugin.Owners.Contains(context.User.Identity.Name)) {
        return true;
      }
    }

    return false;
  }
}