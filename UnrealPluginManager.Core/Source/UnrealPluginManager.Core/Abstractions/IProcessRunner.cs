namespace UnrealPluginManager.Core.Abstractions;

/// <summary>
/// Represents a contract for executing external processes with a specified command and arguments.
/// </summary>
public interface IProcessRunner {
  /// <summary>
  /// Executes an external process with the specified command and arguments.
  /// </summary>
  /// <param name="command">The command or executable file name to run.</param>
  /// <param name="arguments">An array of arguments to pass to the command.</param>
  /// <param name="workingDirectory"></param>
  /// <returns>A task that represents the asynchronous operation and wraps the exit code of the process.</returns>
  Task<int> RunProcess(string command, string[] arguments, string? workingDirectory = null);
}