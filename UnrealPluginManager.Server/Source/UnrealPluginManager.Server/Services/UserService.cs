using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Database.Entities.Users;
using UnrealPluginManager.Core.Mappers;
using UnrealPluginManager.Core.Model.Users;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Server.Services;

/// <summary>
/// Provides user-related services and operations necessary for the
/// Unreal Plugin Manager application.
/// </summary>
[AutoConstructor]
public partial class UserService : IUserService {
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly UnrealPluginManagerContext _dataContext;

  /// <inheritdoc />
  public async Task<UserOverview> GetActiveUser() {
    var principal = _httpContextAccessor.HttpContext?.User;
    principal.RequireNonNull();
    var claimsDict = principal.Claims.ToDictionary(x => x.Type, x => x.Value);
    var username = principal.Identity?.Name;
    
    var existingUser = await _dataContext.Users
        .Where(x => x.Username == username)
        .ToUserOverviewQuery()
        .FirstOrDefaultAsync();
    if (existingUser is not null) {
      return existingUser;
    }

    var newUser = new User {
        Username = username,
        Email = claimsDict[ClaimTypes.Email]
    }; 
    _dataContext.Users.Add(newUser);
    await _dataContext.SaveChangesAsync();
    
    return newUser.ToUserOverview();
  }
}