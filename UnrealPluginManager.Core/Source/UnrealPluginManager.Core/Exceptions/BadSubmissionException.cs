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
public class BadSubmissionException(string? message = null, Exception? innerException = null)
    : BadArgumentException(message, innerException);