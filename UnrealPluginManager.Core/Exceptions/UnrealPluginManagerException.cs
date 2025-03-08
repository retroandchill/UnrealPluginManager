namespace UnrealPluginManager.Core.Exceptions;

/// <summary>
/// Represents a base exception class for errors that occur during
/// the operation of the Unreal Plugin Manager.
/// </summary>
/// <remarks>
/// UnrealPluginManagerException serves as an abstract base class for
/// specific exceptions related to the Unreal Plugin Manager functionality.
/// Derived exceptions can target specific error conditions and provide
/// more meaningful error messages.
/// </remarks>
/// <example>
/// This class cannot be instantiated directly. Use one of its derived
/// classes such as PluginNotFoundException, DependencyResolutionException,
/// or BadSubmissionException for specific error scenarios.
/// </example>
public abstract class UnrealPluginManagerException(string? message = null, Exception? innerException = null)
    : Exception(message, innerException);