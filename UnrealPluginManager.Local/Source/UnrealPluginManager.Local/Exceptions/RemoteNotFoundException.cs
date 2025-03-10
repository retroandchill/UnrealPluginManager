using UnrealPluginManager.Core.Exceptions;

namespace UnrealPluginManager.Local.Exceptions;

public class RemoteNotFoundException(string? message = null, Exception? innerException = null) : UnrealPluginManagerException(message, innerException) {
  
}