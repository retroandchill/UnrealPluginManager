using System.IO.Abstractions;

namespace UnrealPluginManager.Core.Model.Storage;

/// <summary>
/// Represents a stored version of a plugin, including its source file, optional icon, and binaries.
/// </summary>
/// <remarks>
/// This class is used to encapsulate information related to a specific version of a plugin.
/// It includes metadata such as the source file location, an optional icon, and a list of binary files.
/// </remarks>
public class StoredPluginVersion {
  /// <summary>
  /// Gets or sets the source file information for the plugin version.
  /// This property represents the main file associated with the plugin version,
  /// typically used for plugin distribution or storage purposes.
  /// </summary>
  public required IFileInfo Source { get; set; }

  /// <summary>
  /// Gets or sets the icon file associated with the plugin version.
  /// This property represents an optional visual representation for the plugin
  /// that can be used for display purposes in user interfaces or plugin management systems.
  /// </summary>
  public IFileInfo? Icon { get; set; }

  /// <summary>
  /// Gets or sets the collection of binaries associated with the plugin version.
  /// Each binary represents a specific build of the plugin for a given engine version and platform.
  /// This property allows access to the plugin's compiled files for various configurations.
  /// </summary>
  public List<StoredBinary> Binaries { get; set; }
  
}