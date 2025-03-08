namespace UnrealPluginManager.Core.Exceptions;

/// <summary>
/// Exception thrown when one or more required plugin dependencies are missing.
/// </summary>
/// <remarks>
/// This exception is specifically designed to indicate the absence of required
/// dependencies during plugin management operations. It provides detailed information
/// about which plugins are missing, aiding in debugging and resolution of dependency issues.
/// </remarks>
/// <example>
/// Instances of this exception are typically thrown by methods that resolve or verify
/// plugin dependencies within the Unreal Plugin Manager. For example, the PluginService
/// may throw this exception if unresolved dependencies are detected in a dependency manifest.
/// </example>
public class MissingDependenciesException(IEnumerable<string> pluginNames, Exception? innerException = null)
    : UnrealPluginManagerException(GetMessage(pluginNames), innerException) {

  private static string GetMessage(IEnumerable<string> pluginNames) {
    return $"The following plugins could not be found: {string.Join(", ", pluginNames)}";
  }
  
}