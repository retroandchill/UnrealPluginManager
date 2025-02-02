using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database.Entities.Plugins;

namespace UnrealPluginManager.Core.Database.Entities.Engine;

public class EngineVersion {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }
    
    [Required]
    public uint Major { get; set; }
    
    [Required]
    public uint Minor { get; set; }
    
    public ulong PluginId { get; set; }
    
    public Plugin Plugin { get; set; }

    public EngineVersion() {
        
    }

    public EngineVersion(Version version) {
        Major = (uint)version.Major;
        Minor = (uint)version.Minor;
    }

    internal static void DefineModelMetadata(ModelBuilder modelBuilder) {
        modelBuilder.Entity<EngineVersion>()
            .HasOne(x => x.Plugin)
            .WithMany(x => x.CompatibleEngineVersions)
            .HasForeignKey(x => x.PluginId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<EngineVersion>()
            .HasIndex(x => new { x.PluginId, x.Major, x.Minor })
            .IsUnique();
    }
    
    
}