using Microsoft.AspNetCore.Authorization;
using UnrealPluginManager.Core.Model.Users;

namespace UnrealPluginManager.Server.Services;

/// <summary>
/// Defines a service that provides user-related operations.
/// </summary>
public interface IUserService {
  /// <summary>
  /// Retrieves the currently active user based on the context of the HTTP request.
  /// If the user is not found, a new user is created and stored in the database.
  /// </summary>
  /// <returns>
  /// A <see cref="UserOverview"/> object that contains details of the active user.
  /// </returns>
  Task<UserOverview> GetActiveUser();
}