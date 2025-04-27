using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UnrealPluginManager.Core.Model.Users;
using UnrealPluginManager.Server.Auth;
using UnrealPluginManager.Server.Services;

namespace UnrealPluginManager.Server.Controllers;

/// <summary>
/// UsersController provides endpoints for user-related operations in the Unreal Plugin Manager.
/// </summary>
[ApiController]
[Route("users")]
[AutoConstructor]
public partial class UsersController : ControllerBase {
  private readonly IUserService _userService;

  /// Retrieves the currently active user.
  /// <returns>
  /// A task that represents the asynchronous operation.
  /// The task result contains the active user's overview, including details like Id, Username, Email,
  /// and ProfilePicture if available.
  /// </returns>
  [Authorize]
  [HttpGet("active")]
  public Task<UserOverview> GetActiveUser() {
    return _userService.GetActiveUser();
  }

  [Authorize(AuthorizationPolicies.CallingUser)]
  [HttpPost("{userId:guid}/api-keys")]
  public Task<string> CreateApiKey([FromRoute] Guid userId, [FromBody] ApiKeyOverview apiKey) {
    return _userService.CreateApiKey(userId, apiKey);
  }
  
}