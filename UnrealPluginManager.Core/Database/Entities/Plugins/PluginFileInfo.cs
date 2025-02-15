using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Abstractions;
using Microsoft.EntityFrameworkCore;
using Semver;

namespace UnrealPluginManager.Core.Database.Entities.Plugins;

/// <summary>
/// Represents metadata for a plugin file within the database context.
/// </summary>
/// <remarks>
/// Provides detailed information about a specific plugin file, including its path,
/// the engine version it targets, and its relationship with the parent plugin.
/// It is used to manage and store plugin files associated with plugins in the system.
/// </remarks>
public class PluginFileInfo {
    /// <summary>
    /// Gets or sets the unique identifier for the plugin file in the database.
    /// </summary>
    /// <remarks>
    /// The identifier is automatically generated upon creation and serves as the primary key
    /// for the plugin file entry in the database. It uniquely distinguishes each plugin file record.
    /// </remarks>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the parent plugin associated with the plugin file.
    /// </summary>
    /// <remarks>
    /// This property serves as a foreign key linking the plugin file to its parent plugin.
    /// It establishes a relationship between the plugin file and the plugin it belongs to, enabling
    /// hierarchical organization and retrieval of plugin data. The value is required for database storage.
    /// </remarks>
    [Required]
    public ulong ParentId { get; set; }

    /// <summary>
    /// Gets or sets the parent plugin associated with this plugin file.
    /// </summary>
    /// <remarks>
    /// Represents the reference to the parent plugin entity to which this plugin file belongs.
    /// This association is used to establish a relationship between the plugin file
    /// and its parent plugin within the database context.
    /// </remarks>
    public Plugin Parent { get; set; }

    /// <summary>
    /// Gets or sets the file path associated with the plugin file.
    /// </summary>
    /// <remarks>
    /// The file path represents the physical location of the plugin file on the file system.
    /// It is required and stored using an abstraction to support flexible file system operations.
    /// </remarks>
    [Required]
    public required IFileInfo FilePath { get; set; }

    /// <summary>
    /// Gets or sets the Unreal Engine version compatible with the plugin file.
    /// </summary>
    /// <remarks>
    /// This property indicates the specific version of the Unreal Engine with which the plugin file is designed to work.
    /// It is used for ensuring compatibility when managing or retrieving plugin files and is a required property.
    /// </remarks>
    [Required]
    public Version EngineVersion { get; set; } = new(5, 5);

    internal static void DefineModelMetadata(ModelBuilder modelBuilder, IFileSystem filesystem) {
        modelBuilder.Entity<PluginFileInfo>()
            .HasOne(x => x.Parent)
            .WithMany(x => x.UploadedPlugins)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PluginFileInfo>()
            .HasIndex(x => x.ParentId);

        modelBuilder.Entity<PluginFileInfo>()
            .Property(x => x.FilePath)
            .HasConversion(x => x.FullName, x => filesystem.FileInfo.New(x));

        modelBuilder.Entity<PluginFileInfo>()
            .HasIndex(x => x.EngineVersion);

        modelBuilder.Entity<PluginFileInfo>()
            .Property(x => x.EngineVersion)
            .HasConversion(x => x.ToString(), x => Version.Parse(x));
    }
}