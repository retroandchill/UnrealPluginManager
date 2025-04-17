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
  
  /// <summary>
  /// Creates a new API key for a specified user with the given API key details.
  /// </summary>
  /// <param name="userId">The unique identifier of the user for whom the API key is being created.</param>
  /// <param name="apiKey">The details of the API key to be created.</param>
  /// <returns>A string representation of the new API key, including both public and private components.</returns>
  Task<string> CreateApiKey(Guid userId, ApiKeyOverview apiKey);
}