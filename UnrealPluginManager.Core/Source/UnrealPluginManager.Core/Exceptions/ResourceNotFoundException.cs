namespace UnrealPluginManager.Core.Exceptions;

/// <summary>
/// Exception thrown when a specified resource is not found.
/// </summary>
/// <remarks>
/// ResourceNotFoundException is a specific exception that derives from
/// UnrealPluginManagerException. It is used to indicate that certain
/// resources, such as files, directories, or other dependencies, could
/// not be located as part of an operation within the Unreal Plugin Manager.
/// </remarks>
/// <seealso cref="UnrealPluginManagerException"/>
public class ResourceNotFoundException(string? message = null, Exception? innerException = null)
    : ContentNotFoundException(message, innerException);