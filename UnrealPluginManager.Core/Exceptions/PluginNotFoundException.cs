namespace UnrealPluginManager.Core.Exceptions;

public class PluginNotFoundException : UnrealPluginManagerException {
    public PluginNotFoundException() {
        
    }
    
    public PluginNotFoundException(string? message) : base(message) {
        
    }
    
    public PluginNotFoundException(string? message, Exception? innerException) : base(message, innerException) {
        
    }
}