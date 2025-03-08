using System.CommandLine;
using System.CommandLine.IO;
using UnrealPluginManager.Core.Exceptions;

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
[ExceptionHandler(DefaultHandlerMethod = nameof(HandleGenericException))]
[AutoConstructor]
public partial class CliExceptionHandler {
  private readonly IConsole _console;

  private int HandleConflicts(DependencyConflictException dependencyConflictException) {
    _console.Out.WriteLine($"{dependencyConflictException.Message}");
    foreach (var conflict in dependencyConflictException.Conflicts) {
      _console.Out.WriteLine($"\n{conflict.PluginName} required by:");
      foreach (var requiredBy in conflict.Versions) {
        _console.Out.WriteLine($"    {requiredBy.RequiredBy} => {requiredBy.RequiredVersion}");
      }
    }
    return -1;
  }
  
  private int HandleGenericException(Exception ex) {
    _console.Error.WriteLine(ex.Message);
    return 1;
  }

}