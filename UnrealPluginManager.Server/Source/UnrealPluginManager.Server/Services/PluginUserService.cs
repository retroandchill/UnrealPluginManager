using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Server.Database;
using UnrealPluginManager.Server.Database.Users;
using UnrealPluginManager.Server.Model.Plugins;

namespace UnrealPluginManager.Server.Services;

/// <summary>
/// Service responsible for managing user interactions with plugins, including
/// assigning initial ownership to a plugin. Implements <see cref="IPluginUserService"/>.
/// </summary>
[AutoConstructor]
public partial class PluginUserService : IPluginUserService {
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly CloudUnrealPluginManagerContext _dbContext;

  /// <inheritdoc />
  public async Task<bool> AssignInitialOwnershipOfPlugin(Guid pluginId) {
    var username = _httpContextAccessor.HttpContext?.User.Identity?.Name;
    if (username is null) {
      return false;
    }

    var userId = await _dbContext.Users
        .Where(x => x.Username == username)
        .Select(x => (Guid?)x.Id)
        .FirstOrDefaultAsync();
    if (!userId.HasValue) {
      return false;
    }

    if (!await _dbContext.UserPlugins
            .AnyAsync(x => x.PluginId == pluginId)) {
      throw new BadArgumentException("Can only assign initial ownership to a plugin with no pre-existing owners");
    }

    var userPlugin = new UserPlugin {
        PluginId = pluginId,
        UserId = userId.Value,
        Role = UserPluginRole.Owner
    };
    _dbContext.UserPlugins.Add(userPlugin);
    await _dbContext.SaveChangesAsync();
    return true;
  }
}