using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Semver;

namespace UnrealPluginManager.Core.Database.Entities.Plugins;

public class PluginVersion {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }
    
    public ulong ParentId { get; set; }
    
    public Plugin Parent { get; set; }
    
    [NotMapped]
    public SemVersion Version { get; set; } = new(1, 0, 0);
    
    [MinLength(1)]
    [MaxLength(255)]
    [Column(name: "Version")]
    public string VersionString {
        get => Version.ToString();
        set => Version = SemVersion.Parse(value);
    }
    
    public ICollection<Dependency> Dependencies { get; set; } = new List<Dependency>();
    
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