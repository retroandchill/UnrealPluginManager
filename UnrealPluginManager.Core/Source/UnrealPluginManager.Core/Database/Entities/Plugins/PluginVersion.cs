using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Semver;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Database.Entities.Plugins;

/// <summary>
/// Represents a specific version of a plugin within the Unreal Plugin Manager system.
/// This class defines the structure and behavior of a plugin version, including its
/// relationship to the parent plugin, its version details, dependencies, and associated binaries.
/// This entity is associated with a database table and supports Entity Framework Core.
/// </summary>
public class PluginVersion : IVersionedEntity {
  /// <summary>
  /// Gets or sets the unique identifier for the plugin version.
  /// </summary>
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the unique identifier for the parent entity associated with the plugin version.
  /// </summary>
  public Guid ParentId { get; set; }

  /// <summary>
  /// Gets or sets the parent plugin associated with the plugin version.
  /// </summary>
  public Plugin Parent { get; set; }

  /// <summary>
  /// Gets or sets the semantic version of the plugin.
  /// </summary>
  [NotMapped]
  public SemVersion Version {
    get => new(Major, Minor, Patch,
               PrereleaseNumber is not null ? [Prerelease ?? "rc", PrereleaseNumber.Value.ToString()] : null,
               Metadata?.Split('.'));
    set {
      Major = (int)value.Major;
      Minor = (int)value.Minor;
      Patch = (int)value.Patch;
      PrereleaseNumber = value.IsPrerelease ? value.PrereleaseIdentifiers.GetPrereleaseNumber() : null;
      Prerelease = value.IsPrerelease ? value.PrereleaseIdentifiers[0].Value : null;
      Metadata = value.Metadata != "" ? value.Metadata : null;
    }
  }

  /// <summary>
  /// Gets the string representation of the semantic version for the plugin version.
  /// This property combines the major, minor, patch, prerelease, and metadata components
  /// into a single readable version string.
  /// </summary>
  [MaxLength(255)]
  public string VersionString {
    get => Version.ToString();
    private set {
      // Hack for a write-only column
    }
  }

  /// <summary>
  /// Gets or sets the major version number of the plugin version.
  /// </summary>
  public int Major { get; private set; }

  /// <summary>
  /// Gets or sets the minor version number of the plugin.
  /// </summary>
  public int Minor { get; private set; }

  /// <summary>
  /// Gets or sets the patch version number of the plugin.
  /// This is a part of the semantic versioning system (major.minor.patch) used to define the plugin version.
  /// </summary>
  public int Patch { get; private set; }

  /// <summary>
  /// Gets or sets the prerelease label for the plugin version.
  /// This property represents the optional pre-release identifier associated with
  /// the semantic version of the plugin, such as "alpha", "beta", or "rc".
  /// </summary>
  public string? Prerelease { get; private set; }

  /// <summary>
  /// Gets or sets the release candidate number for the plugin version.
  /// This property indicates the pre-release state of the software
  /// and is used in conjunction with semantic versioning.
  /// </summary>
  public int? PrereleaseNumber { get; private set; }

  /// <summary>
  /// Gets or sets the metadata associated with the plugin version.
  /// This typically stores additional version information in a dot-separated format,
  /// which can include build information or other custom descriptors.
  /// </summary>
  [MaxLength(255)]
  public string? Metadata { get; private set; }

  /// <summary>
  /// Gets or sets the collection of dependencies for the plugin version.
  /// </summary>
  public ICollection<Dependency> Dependencies { get; set; } = new List<Dependency>();

  /// <summary>
  /// Gets or sets the collection of binaries associated with the plugin version.
  /// </summary>
  public ICollection<UploadedBinaries> Binaries { get; set; } = new List<UploadedBinaries>();


  internal static void DefineModelMetadata(ModelBuilder modelBuilder) {
    modelBuilder.Entity<PluginVersion>()
        .HasOne(x => x.Parent)
        .WithMany(x => x.Versions)
        .HasForeignKey(x => x.ParentId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Cascade);

    modelBuilder.Entity<PluginVersion>()
        .HasIndex(x => x.ParentId);

    modelBuilder.Entity<PluginVersion>()
        .Ignore(x => x.Version);
  }
}