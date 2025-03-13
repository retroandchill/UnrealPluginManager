using System.IO.Abstractions;
using Dunet;

namespace UnrealPluginManager.Core.Model.Storage;

/// <summary>
/// Represents information about a plugin file, including its type and associated metadata.
/// </summary>
[Union]
public abstract partial record PluginFileInfo {

  /// <summary>
  /// Gets the file information for a plugin file, encapsulating file metadata
  /// such as name, path, and size or permissions depending on the platform.
  /// </summary>
  public IFileInfo File { get; }

  /// <summary>
  /// Represents metadata about a plugin file and serves as the base record for plugin file information types.
  /// </summary>
  private PluginFileInfo(IFileInfo file) {
    File = file;
  }

  /// <summary>
  /// Represents a source file associated with a plugin, containing relevant metadata and file information.
  /// </summary>
  public partial record Source {
    /// <summary>
    /// Represents an abstract base record for plugin file information such as source, binaries, or icon files.
    /// </summary>
    public Source(IFileInfo file) : base(file) {
      
    }
  }

  /// <summary>
  /// Represents an icon file associated with a plugin, containing relevant metadata and file information.
  /// </summary>
  public partial record Icon {
    /// <summary>
    /// Represents an icon file associated with a plugin, containing relevant metadata and file information.
    /// </summary>
    public Icon(IFileInfo file) : base(file) {
      
    }
  }

  /// <summary>
  /// Represents binary data for a plugin, including platform and engine version information.
  /// </summary>
  public partial record Binaries {
    /// <summary>
    /// Gets the target platform associated with the plugin binaries,
    /// typically used to identify the operating system or hardware architecture
    /// the binaries are built for.
    /// </summary>
    public string Platform { get; }
    /// <summary>
    /// Gets the engine version associated with the plugin binaries.
    /// This property is used to categorize and manage binaries based on
    /// their compatibility with specific versions of the Unreal Engine.
    /// </summary>
    public string EngineVersion { get; }

    /// <summary>
    /// Represents binary data associated with a plugin, including platform and engine version metadata.
    /// </summary>
    public Binaries(string engineVersion, string platform, IFileInfo file) : base(file) {
      EngineVersion = engineVersion;
      Platform = platform;
    }
  }

}