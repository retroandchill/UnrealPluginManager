using System.CommandLine;
using System.CommandLine.IO;
using UnrealPluginManager.Core.Exceptions;

namespace UnrealPluginManager.Cli.Exceptions;

[ExceptionHandler(DefaultHandlerMethod = nameof(HandleGenericException))]
[AutoConstructor]
public partial class CliExceptionHandler : ICliExceptionHandler {
  private readonly IConsole _console;

  private int HandleConflicts(DependencyConflictException dependencyConflictException) {
    _console.Out.WriteLine($"${dependencyConflictException}");
    _console.Out.WriteLine("Conflicts:");
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