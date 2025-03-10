using System.IO.Abstractions;
using System.IO.Compression;
using LanguageExt;
using Semver;
using UnrealPluginManager.Core.Model.Storage;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Provides services for managing and partitioning Unreal Engine plugins.
/// </summary>
public interface IPluginStructureService {
  /// <summary>
  /// Partitions a plugin into its source, icon, and binary components and stores them using the storage service.
  /// </summary>
  /// <param name="pluginName">The name of the plugin to be partitioned.</param>
  /// <param name="version">The version of the plugin.</param>
  /// <param name="engineVersion">The version of the Unreal Engine.</param>
  /// <param name="pluginDirectory">The directory containing the plugin's files.</param>
  /// <returns>A <see cref="PartitionedPlugin"/> record containing the plugin's source, optional icon, and binaries.
  /// </returns>
  Task<PartitionedPlugin> PartitionPlugin(string pluginName, SemVersion version, string engineVersion,
                                          IDirectoryInfo pluginDirectory);

  /// <summary>
  /// Partitions a plugin from a ZIP archive into its source, optional icon, and binary components and stores them using the storage service.
  /// </summary>
  /// <param name="pluginName">The name of the plugin to be partitioned.</param>
  /// <param name="version">The semantic version of the plugin.</param>
  /// <param name="engineVersion">The version of the Unreal Engine.</param>
  /// <param name="zipArchive">The ZIP archive containing the plugin's files to be partitioned.</param>
  /// <returns>A <see cref="PartitionedPlugin"/> record containing the plugin's source, optional icon, and binaries.</returns>
  Task<PartitionedPlugin> PartitionPlugin(string pluginName, SemVersion version, string engineVersion,
                                          ZipArchive zipArchive);
  /// <summary>
  /// Retrieves a list of installed binary directories for a given plugin.
  /// </summary>
  /// <param name="pluginDirectory">The directory containing the plugin's files.</param>
  /// <returns>A list of strings representing the paths to the installed binaries.</returns>
  List<string> GetInstalledBinaries(IDirectoryInfo pluginDirectory);
}