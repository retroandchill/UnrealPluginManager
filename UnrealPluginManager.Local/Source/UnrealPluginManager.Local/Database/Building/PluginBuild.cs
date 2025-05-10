using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnrealPluginManager.Core.Database.Entities.Plugins;

namespace UnrealPluginManager.Server.Database.Building;

public class PluginBuild {

  [Key]
  public Guid Id { get; set; }

  public PluginVersion PluginVersion { get; set; } = null!;

  public Guid PluginVersionId { get; set; }

  [MaxLength(255)]
  public required string EngineVersion { get; set; }

  public ICollection<PluginBuildPlatform> Platforms { get; set; } = [];

  public required string DirectoryName { get; set; }

  public DateTimeOffset BuiltOn { get; set; } = DateTimeOffset.Now;

  public ICollection<DependencyBuildVersion> BuiltWith { get; set; } = [];

  internal static void DefineMetamodelData(EntityTypeBuilder<PluginBuild> builder) {
    builder.HasOne(pb => pb.PluginVersion)
        .WithMany()
        .HasForeignKey(pb => pb.PluginVersionId)
        .IsRequired();

    builder.HasMany(pb => pb.Platforms)
        .WithOne(pb => pb.Build)
        .HasForeignKey(pb => pb.BuildId)
        .IsRequired();
  }
}