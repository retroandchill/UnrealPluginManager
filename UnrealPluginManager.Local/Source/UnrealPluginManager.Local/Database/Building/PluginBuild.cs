using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnrealPluginManager.Core.Database.Entities.Plugins;

namespace UnrealPluginManager.Local.Database.Building;

/// <summary>
/// Represents a build of a plugin, including metadata such as engine version,
/// directory structure, platform compatibility, and dependencies.
/// </summary>
public class PluginBuild {
  /// <summary>
  /// Gets or sets the unique identifier for the plugin build. This property serves
  /// as the primary key in the database and is used to uniquely identify a specific
  /// build of a plugin.
  /// </summary>
  [Key]
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the plugin version associated with this build. This property links
  /// the build to a specific version of the plugin, allowing identification of features,
  /// changes, and dependencies tied to that version.
  /// </summary>
  public PluginVersion PluginVersion { get; set; } = null!;

  /// <summary>
  /// Gets or sets the unique identifier corresponding to the plugin version associated
  /// with this build. This identifier links the build to a specific version of the plugin.
  /// </summary>
  public Guid PluginVersionId { get; set; }

  /// <summary>
  /// Gets or sets the version of the engine associated with the plugin build. This property is used
  /// to specify and identify the particular version of the engine with which the plugin build is compatible.
  /// </summary>
  [MaxLength(255)]
  public required string EngineVersion { get; set; }

  /// <summary>
  /// Gets or sets the collection of platform configurations associated with the plugin build.
  /// This property links the build to the platforms it supports.
  /// </summary>
  public ICollection<PluginBuildPlatform> Platforms { get; set; } = new List<PluginBuildPlatform>();

  /// <summary>
  /// Gets or sets the name of the directory where the plugin build is stored.
  /// This property specifies the relative or absolute path to the directory
  /// containing the plugin files for this build.
  /// </summary>
  public required string DirectoryName { get; set; }

  /// <summary>
  /// Gets or sets the date and time when the plugin build was created.
  /// This property is used to track and order builds based on their creation timestamps.
  /// </summary>
  public DateTimeOffset BuiltOn { get; set; } = DateTimeOffset.Now;

  /// <summary>
  /// Gets or sets the collection of dependencies that this plugin build was built with.
  /// This property represents a relationship to the versions of dependencies used
  /// during the build process of the plugin.
  /// </summary>
  public ICollection<DependencyBuildVersion> BuiltWith { get; set; } = new List<DependencyBuildVersion>();

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