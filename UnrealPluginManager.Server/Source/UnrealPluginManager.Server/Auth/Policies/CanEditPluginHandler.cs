using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Retro.ReadOnlyParams.Annotations;
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
public class CanEditPluginHandler(
    [ReadOnly] IHttpContextAccessor httpContextAccessor,
    [ReadOnly] UnrealPluginManagerContext dbContext,
    [ReadOnly] IPluginAuthValidator pluginAuthValidator) : GeneralAuthorizationHandler<CanEditPluginRequirement> {
  /// <inheritdoc />
  protected override async Task HandleInternal(AuthorizationHandlerContext context,
                                               CanEditPluginRequirement requirement) {
    using var usingEvaluation = new ContextEvaluation(context, requirement);
    var httpContext = httpContextAccessor.HttpContext;

    if (httpContext is null || !httpContext.Request.HasFormContentType ||
        context.User.Identity?.IsAuthenticated != true ||
        !httpContext.Request.RouteValues.TryGetValue("pluginId", out var id) || id is not string pluginIdString) {
      context.Fail();
      return;
    }

    var pluginId = Guid.Parse(pluginIdString);

    var pluginName = await dbContext.Plugins
        .Where(x => x.Id == pluginId)
        .Select(x => x.Name)
        .FirstOrDefaultAsync();
    if (pluginName is null) {
      return;
    }

    if (!await pluginAuthValidator.CanEditPlugin(context, pluginName)) {
      context.Fail();
    }
  }
}