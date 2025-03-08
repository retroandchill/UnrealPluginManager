namespace UnrealPluginManager.Core.Abstractions;

/// <summary>
/// Defines methods for interacting with environment variables and special folder paths.
/// </summary>
public interface IEnvironment {
  /// <summary>
  /// Retrieves the value of the specified environment variable from the current process.
  /// If the environment variable does not exist, returns null.
  /// </summary>
  /// <param name="variable">The name of the environment variable to retrieve.</param>
  /// <returns>The value of the environment variable, or null if the environment variable does not exist.</returns>
  string? GetEnvironmentVariable(string variable);

  /// <summary>
  /// Retrieves the path to the specified system special folder identified by the <see cref="Environment.SpecialFolder"/> enumeration.
  /// </summary>
  /// <param name="folder">An enumeration value that identifies the system special folder.</param>
  /// <returns>The path to the specified system special folder.</returns>
  string GetFolderPath(Environment.SpecialFolder folder);
}