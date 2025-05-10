using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Semver;
using UnrealPluginManager.Core.Database.Entities.Plugins;

namespace UnrealPluginManager.Server.Database.Building;

public class DependencyBuildVersion {

  public Guid BuildId { get; set; }

  public PluginBuild Build { get; set; } = null!;

  public Guid DependencyId { get; set; }

  public Dependency Dependency { get; set; } = null!;

  public SemVersion Version { get; set; }

  internal static void DefineMetamodelData(EntityTypeBuilder<DependencyBuildVersion> builder) {
    builder.HasKey(db => new {
        db.BuildId,
        db.DependencyId
    });

    builder.HasOne(x => x.Build)
        .WithMany(x => x.BuiltWith)
        .HasForeignKey(x => x.BuildId)
        .IsRequired();

    builder.HasOne(x => x.Dependency)
        .WithMany()
        .HasForeignKey(x => x.DependencyId)
        .IsRequired();

    builder.Property(x => x.Version)
        .HasConversion(x => x.ToString(), x => SemVersion.Parse(x, 255))
        .HasMaxLength(255);
  }
}