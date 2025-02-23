using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Semver;
using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Database.Entities.Plugins;

/// <summary>
/// Represents a specific version of a plugin within the Unreal Plugin Manager system.
/// This class defines the structure and behavior of a plugin version, including its
/// relationship to the parent plugin, its version details, dependencies, and associated binaries.
/// This entity is associated with a database table and supports Entity Framework Core.
/// </summary>
public class PluginVersion {
    /// <summary>
    /// Gets or sets the unique identifier for the plugin version.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier for the parent entity associated with the plugin version.
    /// </summary>
    public ulong ParentId { get; set; }

    /// <summary>
    /// Gets or sets the parent plugin associated with the plugin version.
    /// </summary>
    public Plugin Parent { get; set; }

    /// <summary>
    /// Gets or sets the semantic version of the plugin.
    /// </summary>
    [NotMapped]
    public SemVersion Version { get; set; } = new(1, 0, 0);

    /// <summary>
    /// Gets or sets the string representation of the plugin version.
    /// </summary>
    [MinLength(1)]
    [MaxLength(255)]
    [Column(name: "Version")]
    public string VersionString {
        get => Version.ToString();
        set => Version = SemVersion.Parse(value);
    }

    /// <summary>
    /// Gets or sets the collection of dependencies for the plugin version.
    /// </summary>
    public ICollection<Dependency> Dependencies { get; set; } = new List<Dependency>();

    /// <summary>
    /// Gets or sets the collection of binaries associated with the plugin version.
    /// </summary>
    public ICollection<PluginBinaries> Binaries { get; set; } = new List<PluginBinaries>();
 
    
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
            .HasIndex(x => x.VersionString)
            .IsUnique();

        modelBuilder.Entity<PluginVersion>()
            .Ignore(x => x.Version);
    }
}