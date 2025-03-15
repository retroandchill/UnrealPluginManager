namespace UnrealPluginManager.Core.Exceptions;

/// <summary>
/// Represents an exception that is thrown when specific content is not found.
/// </summary>
/// <remarks>
/// This exception serves as the base class for more specific exceptions indicating
/// the absence of required content within the system. Derived classes might use this
/// exception to convey more precise details about the missing content.
/// </remarks>
/// <example>
/// This exception might be used in scenarios where plugins, resources, or dependencies
/// are not located or accessible in the expected context.
/// </example>
public class ContentNotFoundException(string? message = null, Exception? innerException = null)
    : UnrealPluginManagerException(message, innerException);