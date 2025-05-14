namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Defines a service responsible for managing ownership of plugins,
/// including assigning initial ownership to a plugin.
/// </summary>
public interface IPluginOwnerService {
  /// <summary>
  /// Assigns initial ownership of a plugin to the current user, if no ownership has yet been established.
  /// </summary>
  /// <param name="pluginId">The unique identifier of the plugin for which initial ownership is to be assigned.</param>
  /// <returns>
  /// A task representing the asynchronous operation. The result of the task indicates
  /// whether the initial ownership assignment was successful.
  /// </returns>
  Task<bool> AssignInitialOwnershipOfPlugin(Guid pluginId);
}