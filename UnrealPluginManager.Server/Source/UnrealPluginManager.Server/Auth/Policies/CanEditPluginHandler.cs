using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Server.Auth.Validators;

namespace UnrealPluginManager.Server.Auth.Policies;

/// <summary>
/// Represents a requirement indicating whether a user has permission to update a plugin.
/// </summary>
public class CanEditPluginRequirement : IAuthorizationRequirement;

/// <summary>
/// Authorization handler that determines whether a user has permission to update a plugin.
/// </summary>
[AutoConstructor]
public partial class CanEditPluginHandler : GeneralAuthorizationHandler<CanEditPluginRequirement> {

  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly UnrealPluginManagerContext _dbContext;
  private readonly IPluginAuthValidator _pluginAuthValidator;

  /// <inheritdoc />
  protected override async Task HandleInternal(AuthorizationHandlerContext context,
                                               CanEditPluginRequirement requirement) {
    using var usingEvaluation = new ContextEvaluation(context, requirement);
    var httpContext = _httpContextAccessor.HttpContext;

    if (httpContext is null || !httpContext.Request.HasFormContentType ||
        context.User.Identity?.IsAuthenticated != true ||
        !httpContext.Request.RouteValues.TryGetValue("pluginId", out var id) || id is not string pluginIdString) {
      context.Fail();
      return;
    }
    var pluginId = Guid.Parse(pluginIdString);

    var pluginName = await _dbContext.Plugins
        .Where(x => x.Id == pluginId)
        .Select(x => x.Name)
        .FirstOrDefaultAsync();
    if (pluginName is null) {
      return;
    }

    if (!await _pluginAuthValidator.CanEditPlugin(context, pluginName)) {
      context.Fail();
    }
  }
}