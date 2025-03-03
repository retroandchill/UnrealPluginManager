namespace UnrealPluginManager.Core.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a dependency resolution process fails in the Unreal Plugin Manager.
/// </summary>
/// <remarks>
/// DependencyResolutionException is used to signal errors that occur during
/// the resolution of plugin dependencies. For example, this exception may be raised
/// when the system is unable to resolve one or more plugin names due to conflicts or missing dependencies.
/// This exception extends UnrealPluginManagerException to specialize in dependency-related error handling.
/// </remarks>
public class DependencyResolutionException : UnrealPluginManagerException {
  public DependencyResolutionException() {
  }

  public DependencyResolutionException(string? message) : base(message) {
  }

  public DependencyResolutionException(string? message, Exception? innerException) : base(message, innerException) {
  }
}