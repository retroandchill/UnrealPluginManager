using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnrealPluginManager.Core.Database.Entities.Storage;

namespace UnrealPluginManager.Core.Database.Entities.Plugins;

/// <summary>
/// Represents a patch for a specific plugin version, including information about
/// the associated file resource and patch number.
/// </summary>
public class PluginSourcePatch {
  /// <summary>
  /// Gets or sets the unique identifier of the associated plugin version.
  /// </summary>
  /// <remarks>
  /// This identifier establishes a relationship between the plugin patch and the specific plugin version it applies to.
  /// It is a foreign key linking to the <c>PluginVersion</c> entity, enforcing the correspondence between a patch and its plugin version.
  /// </remarks>
  public Guid PluginVersionId { get; set; }

  /// <summary>
  /// Gets or sets the associated plugin version.
  /// </summary>
  /// <remarks>
  /// This property establishes a navigation relationship to the <c>PluginVersion</c> entity,
  /// representing the specific version of the plugin that this patch is applied to.
  /// It links the patch to its corresponding version, enabling dependency management and version control.
  /// </remarks>
  public PluginVersion PluginVersion { get; set; } = null!;

  /// <summary>
  /// Gets or sets the unique identifier for the associated file resource.
  /// </summary>
  /// <remarks>
  /// This identifier establishes a relationship between the plugin source patch and the specific file resource it references.
  /// It is a foreign key linking to the <c>FileResource</c> entity, ensuring the association of the plugin patch with its corresponding file storage resource.
  /// </remarks>
  public Guid FileResourceId { get; set; }

  /// <summary>
  /// Gets or sets the file resource associated with the plugin patch.
  /// </summary>
  /// <remarks>
  /// This property represents the relationship to a file resource, linked through the <c>FileResourceId</c>.
  /// It contains information about the patch's file data, such as its original and stored filenames, which are used
  /// during storage operations or retrieval processes in the context of plugin version management.
  /// </remarks>
  public FileResource FileResource { get; set; } = null!;

  /// <summary>
  /// Gets or sets the unique sequential number identifying this patch within the context of a specific plugin version.
  /// </summary>
  /// <remarks>
  /// This number is used to determine the order of patches for a particular plugin version. It plays a critical role in ensuring patches are applied and processed in the correct sequence.
  /// The <c>PatchNumber</c> is a component of the composite primary key and must be unique for each patch within the same plugin version.
  /// </remarks>
  public uint PatchNumber { get; set; }

  internal static void DefineModelMetadata(EntityTypeBuilder<PluginSourcePatch> entity) {
    entity.HasKey(x => new {
        x.PluginVersionId,
        x.PatchNumber
    });

    entity.HasOne(x => x.PluginVersion)
        .WithMany(x => x.Patches)
        .HasForeignKey(x => x.PluginVersionId)
        .OnDelete(DeleteBehavior.Cascade)
        .IsRequired();

    entity.HasOne(x => x.FileResource)
        .WithMany()
        .HasForeignKey(x => x.FileResourceId)
        .OnDelete(DeleteBehavior.NoAction)
        .IsRequired();
  }
}