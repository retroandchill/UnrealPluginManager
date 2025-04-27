using Microsoft.AspNetCore.Authorization;

namespace UnrealPluginManager.Server.Auth.Policies;

/// <summary>
/// Represents a generic base authorization handler for handling authorization requirements that implement <see cref="IAuthorizationRequirement"/>.
/// </summary>
/// <typeparam name="T">
/// The type of the authorization requirement. Must implement <see cref="IAuthorizationRequirement"/>.
/// </typeparam>
/// <remarks>
/// The <see cref="GeneralAuthorizationHandler{T}"/> provides a basic structure for handling authorization logic.
/// If the user is in the "Admin" role, the requirement will automatically succeed.
/// Otherwise, subclass implementations must override <see cref="HandleInternal"/> to provide additional
/// requirement-specific authorization logic.
/// </remarks>
public abstract class GeneralAuthorizationHandler<T> : AuthorizationHandler<T> where T : IAuthorizationRequirement {

  /// <inheritdoc />
  protected sealed override Task HandleRequirementAsync(AuthorizationHandlerContext context, T requirement) {
    if (!context.User.IsInRole("Admin")) {
      return HandleInternal(context, requirement);
    }

    context.Succeed(requirement);
    return Task.CompletedTask;
  }

  /// <summary>
  /// Handles the authorization logic for the specific requirement and context.
  /// </summary>
  /// <param name="context">The authorization handler context that contains the information required for evaluating access control policies.</param>
  /// <param name="requirement">The specific requirement being evaluated for authorization.</param>
  /// <returns>A task representing the asynchronous operation.</returns>
  protected abstract Task HandleInternal(AuthorizationHandlerContext context, T requirement);
}