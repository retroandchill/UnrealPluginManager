namespace UnrealPluginManager.Core.Exceptions;

/// <summary>
/// Represents errors that occur due to invalid or unsupported arguments.
/// </summary>
/// <remarks>
/// This exception is a specialized form of <see cref="UnrealPluginManagerException"/> and provides
/// additional context for argument-related errors in the Unreal Plugin Manager.
/// </remarks>
/// <seealso cref="UnrealPluginManagerException"/>
/// <seealso cref="BadSubmissionException"/>
public class BadArgumentException(string? message = null, Exception? innerException = null)
    : UnrealPluginManagerException(message, innerException);