using System.IO.Abstractions;

namespace UnrealPluginManager.Core.Model.Storage;

/// <summary>
/// Represents a plugin that has been partitioned into its source file, an optional icon file, and a collection of binary files.
/// </summary>
/// <remarks>
/// This structure is used to organize the components of a plugin after it has been partitioned using a storage service.
/// It contains references to the main source file of the plugin, an optional icon file (if available),
/// and a dictionary of binary files categorized by platform or other identifying keys.
/// </remarks>
/// <param name="Source">
/// The primary source file of the plugin.
/// </param>
/// <param name="Icon">
/// The optional icon file of the plugin. This can be null if the plugin does not include an icon.
/// </param>
/// <param name="Binaries">
/// A dictionary representing the binary files associated with the plugin.
/// The keys are identifiers (e.g., platform names), and the values are the corresponding binary files.
/// </param>
public record struct PartitionedPlugin(IFileInfo Source, IFileInfo? Icon, Dictionary<string, IFileInfo> Binaries);