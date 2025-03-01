using System.IO.Abstractions;
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

}