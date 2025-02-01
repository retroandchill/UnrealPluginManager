namespace UnrealPluginManager.Core.Exceptions;

public class DependencyResolutionException : UnrealPluginManagerException {
    public DependencyResolutionException() {
        
    }
    
    public DependencyResolutionException(string? message) : base(message) {
        
    }
    
    public DependencyResolutionException(string? message, Exception? innerException) : base(message, innerException) {
        
    }
}