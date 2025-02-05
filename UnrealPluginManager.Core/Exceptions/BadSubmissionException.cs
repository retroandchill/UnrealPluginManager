namespace UnrealPluginManager.Core.Exceptions;

public class BadSubmissionException : UnrealPluginManagerException {
    public BadSubmissionException() {
        
    }
    
    public BadSubmissionException(string? message) : base(message) {
        
    }
    
    public BadSubmissionException(string? message, Exception? innerException) : base(message, innerException) {
        
    }
}