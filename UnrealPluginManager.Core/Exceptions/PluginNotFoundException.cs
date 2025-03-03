namespace UnrealPluginManager.Core.Exceptions;

/// <summary>
/// Represents an exception thrown when a specified plugin cannot be found
/// in the Unreal Plugin Manager's context.
/// </summary>
/// <remarks>
/// This exception is used to signal that an attempt to access a plugin
/// has failed due to the plugin not being located. It could be raised
/// when trying to retrieve plugin data that does not exist or is
/// inaccessible in the system.
/// </remarks>
/// <seealso cref="UnrealPluginManagerException" />
public class PluginNotFoundException : UnrealPluginManagerException {
  public PluginNotFoundException() {
  }

  public PluginNotFoundException(string? message) : base(message) {
  }

  public PluginNotFoundException(string? message, Exception? innerException) : base(message, innerException) {
  }
}