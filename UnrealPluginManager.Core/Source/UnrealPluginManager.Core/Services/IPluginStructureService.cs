using System.IO.Abstractions;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Provides services for managing and partitioning Unreal Engine plugins.
/// </summary>
public interface IPluginStructureService {
  /// <summary>
  /// Retrieves a list of installed binary directories for a given plugin.
  /// </summary>
  /// <param name="pluginDirectory">The directory containing the plugin's files.</param>
  /// <returns>A list of strings representing the paths to the installed binaries.</returns>
  List<string> GetInstalledBinaries(IDirectoryInfo pluginDirectory);
}