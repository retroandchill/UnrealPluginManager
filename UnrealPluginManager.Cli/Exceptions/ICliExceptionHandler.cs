namespace UnrealPluginManager.Cli.Exceptions;

public interface ICliExceptionHandler {

  /// <summary>
  /// Handles an exception and determines the exit code to be returned.
  /// </summary>
  /// <param name="ex">The exception to be handled.</param>
  /// <returns>An integer representing the exit code determined by the exception handling logic.</returns>
  public int HandleException(Exception ex);
  
}