using Retro.SimplePage;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Users;

namespace UnrealPluginManager.Server.Services;

/// <summary>
/// Defines a service that provides user-related operations.
/// </summary>
public interface IUserService {
  /// <summary>
  /// Retrieves the details of a user based on their unique identifier.
  /// </summary>
  /// <param name="userId">The unique identifier of the user whose data is to be retrieved.</param>
  /// <returns>
  /// A <see cref="UserOverview"/> object containing details of the specified user.
  /// </returns>
  Task<UserOverview> GetUser(Guid userId);

  /// <summary>
  /// Retrieves the currently active user based on the context of the HTTP request.
  /// If the user is not found, a new user is created and stored in the database.
  /// </summary>
  /// <returns>
  /// A <see cref="UserOverview"/> object that contains details of the active user.
  /// </returns>
  Task<UserOverview> GetActiveUser();


  /// <summary>
  /// Retrieves a paginated list of plugins associated with a specific user.
  /// </summary>
  /// <param name="userId">The unique identifier of the user whose plugins are to be retrieved.</param>
  /// <param name="pageable">Pagination details specifying the page number and size for the requested data.</param>
  /// <returns>
  /// A <see cref="Page{PluginVersionInfo}"/> object containing a list of plugins associated with the user, along with pagination details.
  /// </returns>
  Task<Page<PluginVersionInfo>> GetUserPlugins(Guid userId, Pageable pageable);

  /// <summary>
  /// Creates a new API key for a specified user with the given API key details.
  /// </summary>
  /// <param name="userId">The unique identifier of the user for whom the API key is being created.</param>
  /// <param name="apiKey">The details of the API key to be created.</param>
  /// <returns>A string representation of the new API key, including both public and private components.</returns>
  Task<string> CreateApiKey(Guid userId, ApiKeyOverview apiKey);
}