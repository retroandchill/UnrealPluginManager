using System.CommandLine;
using System.CommandLine.IO;
using UnrealPluginManager.Local.Model.Installation;

namespace UnrealPluginManager.Cli.Helpers;

/// <summary>
/// Provides extension methods for working with the console in the context of Unreal Plugin Manager,
/// enabling custom output formatting, particularly for displaying plugin version changes.
/// </summary>
public static class ConsoleExtensions {
  /// <summary>
  /// Writes the version changes of plugins to the console output.
  /// </summary>
  /// <param name="console">The console instance that will be used for writing the output.</param>
  /// <param name="changes">A list of version changes containing details about the plugins, including their old and new versions.</param>
  public static void WriteVersionChanges(this IConsole console, List<VersionChange> changes) {
    if (changes.Count == 0) {
      console.Out.WriteLine("No changes detected.");
      return;
    }
    
    console.Out.WriteLine("Successfully installed/upgraded the following plugin(s):");
    foreach (var change in changes) {
      console.Out.WriteLine($"- {change.PluginName}: {change.OldVersion?.ToString() ?? "(new)"} => {change.NewVersion}");
    }
  }
}