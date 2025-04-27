namespace UnrealPluginManager.Core.Exceptions;

/// <summary>
/// Represents an exception that is thrown when an error occurs while interacting with a foreign API.
/// </summary>
/// <remarks>
/// This exception is a specialized form of <see cref="UnrealPluginManagerException"/>,
/// which provides additional context about the error by including the status code returned by the foreign API.
/// </remarks>
public class ForeignApiException(int statusCode, string? message = null, Exception? cause = null) 
    : UnrealPluginManagerException(message, cause) {
  /// <summary>
  /// Gets the status code returned by the foreign API that triggered the exception.
  /// </summary>
  /// <remarks>
  /// This property provides the numerical status code associated with the error,
  /// offering additional diagnostic information when analyzing interactions with the foreign API.
  /// </remarks>
  public int StatusCode { get; } = statusCode;
  
}