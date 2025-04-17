using Microsoft.AspNetCore.Authorization;

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
[AutoConstructor]
public sealed partial class ContextEvaluation : IDisposable {
  private readonly AuthorizationHandlerContext _context;
  private readonly IAuthorizationRequirement _requirement;

  /// <inheritdoc />
  public void Dispose() {
    if (!_context.HasFailed) {
      _context.Succeed(_requirement);
    }
  }
}