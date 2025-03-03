using System.ComponentModel.DataAnnotations;

namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents an overview of binary files for a specific platform and engine version.
/// </summary>
/// <remarks>
/// This class provides details for binary file associations, including the unique
/// identifier of the binary, its target platform, and the corresponding engine version.
/// Used within plugin management to categorize and retrieve binary-specific information.
/// </remarks>
public class BinariesOverview {
  /// <summary>
  /// Gets or sets the unique identifier for the binary file.
  /// </summary>
  /// <remarks>
  /// This property represents a numeric value that uniquely identifies
  /// a specific binary within the system. It is used as the primary identifier
  /// for referencing this binary in various operations.
  /// </remarks>
  [Required]
  [Range(1, ulong.MaxValue)]
  public ulong Id { get; set; }

  /// <summary>
  /// Gets or sets the target platform for which the binary is intended.
  /// </summary>
  /// <remarks>
  /// This property specifies the platform (e.g., Windows, Linux, macOS) associated
  /// with the binary file. It is used to categorize binaries within the application
  /// based on their target deployment environment.
  /// </remarks>
  [Required]
  [MinLength(1)]
  [MaxLength(255)]
  public required string Platform { get; set; }

  /// <summary>
  /// Gets or sets the engine version associated with the binary file or plugin.
  /// </summary>
  /// <remarks>
  /// This property defines the specific version of the Unreal Engine that the binary
  /// is compatible with. It ensures proper classification and compatibility checks
  /// between plugins and engine versions during plugin management operations.
  /// </remarks>
  [Required]
  public required string EngineVersion { get; set; }
}