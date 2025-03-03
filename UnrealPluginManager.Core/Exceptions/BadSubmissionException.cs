namespace UnrealPluginManager.Core.Exceptions;

/// <summary>
/// Represents an exception that is thrown when a plugin submission fails.
/// </summary>
/// <remarks>
/// This exception is used to indicate issues with the submission of plugin data, such as:
/// - The absence of a required .uplugin file.
/// - A malformed or invalid .uplugin file.
/// It is primarily used in scenarios where plugins are submitted for storage or validation.
/// </remarks>
public class BadSubmissionException : UnrealPluginManagerException {
  public BadSubmissionException() {
  }

  public BadSubmissionException(string? message) : base(message) {
  }

  public BadSubmissionException(string? message, Exception? innerException) : base(message, innerException) {
  }
}