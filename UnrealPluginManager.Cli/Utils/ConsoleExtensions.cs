using System.CommandLine;
using System.CommandLine.IO;
using UnrealPluginManager.Core.Model.Resolution;

namespace UnrealPluginManager.Cli.Utils;

/// <summary>
/// Provides extension methods for the <see cref="IConsole"/> interface to facilitate
/// enhanced console interactions and output formatting.
/// </summary>
public static class ConsoleExtensions {
  /// <summary>
  /// Reports plugin dependency conflicts to the console.
  /// </summary>
  /// <param name="console">
  /// The <see cref="IConsole"/> instance used for outputting conflict information.
  /// </param>
  /// <param name="pluginName">
  /// The name of the plugin for which conflicts are being reported.
  /// </param>
  /// <param name="conflicts">
  /// A collection of <see cref="Conflict"/> instances that detail the conflicting plugin requirements.
  /// </param>
  public static void ReportConflicts(this IConsole console, string pluginName, IEnumerable<Conflict> conflicts) {
    console.Out.WriteLine($"Unable to install {pluginName} due conflicts. The following conflicts were detected:");
    foreach (var conflict in conflicts) {
      console.Out.WriteLine($"\n{conflict.PluginName} required by:");
      foreach (var requiredBy in conflict.Versions) {
        console.Out.WriteLine($"    {requiredBy.RequiredBy} => {requiredBy.RequiredVersion}");
      }
    }
  }

}