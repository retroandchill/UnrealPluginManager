using Microsoft.AspNetCore.Authorization;
using Retro.ReadOnlyParams.Annotations;

namespace UnrealPluginManager.Server.Auth;

/// <summary>
/// Represents an evaluation context used for authentication purposes
/// within the UnrealPluginManager server. This class is responsible for
/// managing requirement success based on the provided context.
/// </summary>
/// <remarks>
/// Implements the IDisposable interface to ensure proper cleanup
/// of resources associated with the evaluation context.
/// </remarks>
public sealed class ContextEvaluation(
    [ReadOnly] AuthorizationHandlerContext context,
    [ReadOnly] IAuthorizationRequirement requirement) : IDisposable {
  /// <inheritdoc />
  public void Dispose() {
    if (!context.HasFailed) {
      context.Succeed(requirement);
    }
  }
}