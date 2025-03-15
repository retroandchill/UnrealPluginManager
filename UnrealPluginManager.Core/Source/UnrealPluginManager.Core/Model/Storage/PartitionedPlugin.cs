using UnrealPluginManager.Core.Model.Engine;

namespace UnrealPluginManager.Core.Model.Storage;

/// <summary>
/// Represents a plugin that has been partitioned into its source file, an optional icon file, and a collection of binary files.
/// </summary>
/// <remarks>
/// This structure is used to organize the components of a plugin after it has been partitioned using a storage service.
/// It contains references to the main source file of the plugin, an optional icon file (if available),
/// and a dictionary of binary files categorized by platform or other identifying keys.
/// </remarks>
public record PartitionedPlugin(
    ResourceHandle Source,
    ResourceHandle? Icon,
    ResourceHandle? Readme,
    Dictionary<PluginBinaryType, ResourceHandle> Binaries);