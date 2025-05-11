using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Semver;
using UnrealPluginManager.Core.Database.Entities.Storage;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
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
  public Guid Id { get; set; } = Guid.CreateVersion7();

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
      Major = (int) value.Major;
      Minor = (int) value.Minor;
      Patch = (int) value.Patch;
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
  /// Gets or sets the description of the plugin.
  /// </summary>
  /// <remarks>
  /// This property contains a textual representation that describes the plugin's purpose, functionality, or other relevant details.
  /// It is constrained to a string length between 1 and 2000 characters.
  /// </remarks>
  [MinLength(1)]
  [MaxLength(2000)]
  public string? Description { get; set; }

  /// <summary>
  /// Gets or sets the name of the author of the plugin.
  /// </summary>
  /// <remarks>
  /// This property contains the name of the individual or entity responsible for creating the plugin.
  /// It is optional but must adhere to the specified length and format restrictions if provided.
  /// </remarks>
  [MinLength(1)]
  [MaxLength(255)]
  public string? Author { get; set; }

  /// <summary>
  /// Gets or sets the license information for the plugin version.
  /// This property typically indicates the terms under which the plugin is distributed.
  /// </summary>
  [MaxLength(255)]
  public string? License { get; init; }

  /// <summary>
  /// Gets or sets the URI pointing to the homepage of the plugin version.
  /// The homepage typically contains additional information about the plugin,
  /// such as documentation, updates, and support resources.
  /// </summary>
  [MaxLength(255)]
  public Uri? Homepage { get; set; }

  /// <summary>
  /// Gets or sets the collection of dependencies for the plugin version.
  /// </summary>
  public ICollection<Dependency> Dependencies { get; set; } = new List<Dependency>();

  /// <summary>
  /// Gets or sets the timestamp indicating when the plugin version was created.
  /// </summary>
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

  /// <summary>
  /// Gets or sets the source location associated with the plugin version.
  /// </summary>
  public required SourceLocation Source { get; set; }

  public ICollection<PluginSourcePatch> Patches { get; set; } = new List<PluginSourcePatch>();

  /// <summary>
  /// Gets or sets the associated icon for the plugin version.
  /// This property references a file resource that represents the icon.
  /// </summary>
  public FileResource? Icon { get; set; }

  /// <summary>
  /// Gets or sets the unique identifier of the icon associated with this plugin version.
  /// </summary>
  public Guid? IconId { get; set; }

  /// <summary>
  /// Gets or sets the associated readme file resource for the plugin version.
  /// </summary>
  public FileResource? Readme { get; set; }

  /// <summary>
  /// Gets or sets the unique identifier for the readme file associated with the plugin version.
  /// </summary>
  public Guid? ReadmeId { get; set; }

  internal static void DefineModelMetadata(EntityTypeBuilder<PluginVersion> entity) {
    entity.HasOne(x => x.Parent)
        .WithMany(x => x.Versions)
        .HasForeignKey(x => x.ParentId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasIndex(x => x.ParentId);

    entity.Ignore(x => x.Version);

    entity.ComplexProperty(x => x.Source, sourceBuilder => {
      sourceBuilder.Property(s => s.Url).HasColumnName("source_url");
      sourceBuilder.Property(s => s.Sha).HasColumnName("source_sha");
    });

    entity.HasOne(x => x.Icon)
        .WithMany()
        .HasForeignKey(x => x.IconId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasOne(x => x.Readme)
        .WithMany()
        .HasForeignKey(x => x.ReadmeId)
        .OnDelete(DeleteBehavior.NoAction);
  }
}