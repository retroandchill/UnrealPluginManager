namespace UnrealPluginManager.Core.Exceptions;

public class UnrealPluginManagerException : Exception {
    public UnrealPluginManagerException() {
        
    }
    
    public UnrealPluginManagerException(string? message) : base(message) {
        
    }
    
    public UnrealPluginManagerException(string? message, Exception? innerException) : base(message, innerException) {
        
    }
    
}