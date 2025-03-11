using UnrealPluginManager.Core.Exceptions;

namespace UnrealPluginManager.Local.Exceptions;

/// <summary>
/// The RemoteNotFoundException is thrown when an operation fails due to the absence of a default
/// remote configuration required for the execution of a requested task in the Unreal Plugin Manager.
/// </summary>
/// <remarks>
/// This exception is typically used to indicate misconfiguration or missing setup related to remotes
/// within the plugin management system.
/// </remarks>
/// <param name="message">
/// A custom message that describes the error condition, providing context about what caused the exception.
/// </param>
/// <param name="innerException">
/// An optional inner exception that provides additional details about the higher-level exception.
/// </param>
public class RemoteNotFoundException(string? message = null, Exception? innerException = null) : UnrealPluginManagerException(message, innerException) {
  
}