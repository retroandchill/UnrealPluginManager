using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UnrealPluginManager.Local.Database.Building;

/// <summary>
/// Represents a platform associated with a specific plugin build.
/// This entity links a plugin build to a specific target platform.
/// </summary>
public class PluginBuildPlatform {
  /// <summary>
  /// Represents the unique identifier for the plugin build associated with this platform.
  /// It acts as a foreign key linking the platform to its corresponding plugin build.
  /// </summary>
  public Guid BuildId { get; set; }

  /// <summary>
  /// Represents the plugin build associated with a specific platform.
  /// Establishes the relationship between a platform and its corresponding plugin build entity.
  /// </summary>
  public PluginBuild Build { get; set; } = null!;

  /// <summary>
  /// Defines the specific target platform associated with the plugin build.
  /// It represents the platform for which the plugin is built and compatible, such as Windows, Mac, or Linux.
  /// </summary>
  public required string Platform { get; set; }

  internal static void DefineMetamodelData(EntityTypeBuilder<PluginBuildPlatform> builder) {
    builder.HasKey(pb => new {
        pb.BuildId,
        pb.Platform
    });
  }
}