using System.CommandLine;
using System.CommandLine.IO;
using AutoExceptionHandler.Annotations;
using Retro.ReadOnlyParams.Annotations;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Cli.Exceptions;

/// <summary>
/// Handles exceptions within the CLI application context.
/// </summary>
/// <remarks>
/// The <see cref="CliExceptionHandler"/> class is designed to assist with the
/// graceful handling of exceptions that occur during the execution of CLI commands.
/// This handler uses a default method specified by the <see cref="ExceptionHandlerAttribute"/>
/// to process and handle exceptions.
/// </remarks>
[ExceptionHandler]
public partial class CliExceptionHandler([ReadOnly] IConsole console) {
  /// <summary>
  /// Handles a given exception and returns an appropriate exit code for the CLI application.
  /// </summary>
  /// <param name="ex">The exception instance to handle.</param>
  /// <returns>An integer representing the exit code for the application,
  /// indicating success or the type of error encountered.</returns>
  [GeneralExceptionHandler]
  public partial int HandleException(Exception ex);

  [HandlesException]
  private int HandleConflicts(DependencyConflictException dependencyConflictException) {
    console.Out.WriteLine($"{dependencyConflictException.Message}");
    foreach (var conflict in dependencyConflictException.Conflicts) {
      console.Out.WriteLine($"\n{conflict.PluginName} required by:");
      foreach (var requiredBy in conflict.Versions) {
        console.Out.WriteLine($"    {requiredBy.RequiredBy} => {requiredBy.RequiredVersion}");
      }
    }

    return -1;
  }

  [HandlesException]
  private int HandleNotFound(UnrealPluginManagerException exception) {
    console.Out.WriteLine($"{exception.Message}");
    return 12;
  }

  [HandlesException(typeof(ApiException))]
  private int HandleApiException(ApiException exception) {
    console.Out.WriteLine($"Call to remote server failed with code {exception.ErrorCode}.");
    return 34;
  }

  [FallbackExceptionHandler]
  private int HandleGenericException(Exception ex) {
    console.Error.WriteLine(ex.Message);
    if (ex.StackTrace != null) {
      console.Error.WriteLine(ex.StackTrace);
    }

    return 1;
  }
}