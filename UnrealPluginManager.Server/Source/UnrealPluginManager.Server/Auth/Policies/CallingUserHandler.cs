using LanguageExt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;

namespace UnrealPluginManager.Server.Auth.Policies;

/// <summary>
/// Represents a custom authorization requirement that is used to enforce policies
/// regarding the calling user within the application.
/// </summary>
/// <remarks>
/// This requirement is primarily utilized in scenarios where access control decisions
/// need to ensure that the calling user meets specific criteria. It is associated with
/// a policy and handled by a corresponding <see cref="CallingUserHandler"/> implementation,
/// which evaluates this requirement.
/// </remarks>
public class CallingUserRequirement : IAuthorizationRequirement;

/// <summary>
/// Handles the evaluation of the <see cref="CallingUserRequirement"/> authorization requirement.
/// </summary>
/// <remarks>
/// This handler is responsible for verifying that the calling user corresponds to the expected user context.
/// The evaluation involves checking the authenticity of the user's identity, validating route parameters, and
/// comparing the user information provided in the HTTP context against the data stored in the application’s database.
/// If the conditions are not met, the requirement fails authorization.
/// </remarks>
[AutoConstructor]
public partial class CallingUserHandler : AuthorizationHandler<CallingUserRequirement> {
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly UnrealPluginManagerContext _dbContext;
  
  protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CallingUserRequirement requirement) {
    using var usingEvaluation = new ContextEvaluation(context, requirement);
    var httpContext = _httpContextAccessor.HttpContext;

    if (httpContext is null || context.User.Identity is null || !context.User.Identity.IsAuthenticated ||
        !httpContext.Request.RouteValues.TryGetValue("userId", out var id) || id is not string userIdString) {
      context.Fail();
      return;
    }
    var userId = Guid.Parse(userIdString);

    var username = context.User.Identity.Name;
    var existingUserId = await _dbContext.Users
        .Where(x => x.Username == username)
        .Select(x => x.Id)
        .FirstOrDefaultAsync();
    if (existingUserId != userId) {
      context.Fail();
    }
  }
}