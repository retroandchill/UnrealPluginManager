using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace UnrealPluginManager.Server.Database.Building;

public class PluginBuildPlatform {
  public Guid BuildId { get; set; }
  public PluginBuild Build { get; set; } = null!;

  public required string Platform { get; set; }

  internal static void DefineMetamodelData(EntityTypeBuilder<PluginBuildPlatform> builder) {
    builder.HasKey(pb => new {
        pb.BuildId,
        pb.Platform
    });
  }
}