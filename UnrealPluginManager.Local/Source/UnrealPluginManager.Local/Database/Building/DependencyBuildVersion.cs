using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Semver;
using UnrealPluginManager.Core.Database.Entities.Plugins;

namespace UnrealPluginManager.Local.Database.Building;

/// <summary>
/// Represents the specific version of a dependency associated with a specific plugin build.
/// </summary>
/// <remarks>
/// This class defines a many-to-one relationship between a plugin build and its dependencies,
/// encapsulating the version information for each dependency as it pertains to that build.
/// </remarks>
public class DependencyBuildVersion {
  /// <summary>
  /// Gets or sets the unique identifier of the associated plugin build.
  /// </summary>
  /// <remarks>
  /// Serves as a foreign key for the relationship with the <see cref="PluginBuild"/> entity,
  /// representing the build that includes the specific dependency version.
  /// </remarks>
  public Guid BuildId { get; set; }

  /// <summary>
  /// Gets or sets the plugin build associated with the dependency version.
  /// </summary>
  /// <remarks>
  /// Represents the many-to-one relationship with the <see cref="PluginBuild"/> entity, encapsulating
  /// the context of the specific plugin build that includes the dependency version.
  /// </remarks>
  public PluginBuild Build { get; set; } = null!;

  /// <summary>
  /// Gets or sets the unique identifier of the dependency associated with a plugin build.
  /// </summary>
  /// <remarks>
  /// Represents the relationship between the specific plugin build and a dependency, serving as a key
  /// to identify the dependency within the context of a build.
  /// This property acts as a foreign key to the associated <see cref="Dependency"/> entity.
  /// </remarks>
  public Guid DependencyId { get; set; }

  /// <summary>
  /// Gets or sets the dependency associated with a specific plugin build.
  /// </summary>
  /// <remarks>
  /// Represents the dependency entity related to a plugin build, encapsulating information
  /// about the external resource or component required by the build.
  /// This property establishes a navigation link to the <see cref="Dependency"/> entity,
  /// enabling access to detailed dependency information.
  /// </remarks>
  public Dependency Dependency { get; set; } = null!;

  /// <summary>
  /// Gets or sets the semantic version of the dependency associated with the plugin build.
  /// </summary>
  /// <remarks>
  /// Represents the specific version of a dependency as it pertains to a particular plugin build.
  /// This property is crucial for tracking the exact version of external dependencies utilized
  /// during the build process.
  /// </remarks>
  public SemVersion Version { get; set; }

  internal static void DefineMetamodelData(EntityTypeBuilder<DependencyBuildVersion> builder) {
    builder.HasKey(db => new {
        db.BuildId,
        db.DependencyId
    });

    builder.HasOne(x => x.Build)
        .WithMany(x => x.BuiltWith)
        .HasForeignKey(x => x.BuildId)
        .OnDelete(DeleteBehavior.Cascade)
        .IsRequired();

    builder.HasOne(x => x.Dependency)
        .WithMany()
        .HasForeignKey(x => x.DependencyId)
        .OnDelete(DeleteBehavior.Cascade)
        .IsRequired();

    builder.Property(x => x.Version)
        .HasConversion(x => x.ToString(), x => SemVersion.Parse(x, 255))
        .HasMaxLength(255);
  }
}