using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Semver;

namespace UnrealPluginManager.Core.Database.Entities.Plugins;

public class PluginFileInfo {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }
    
    [Required]
    public ulong ParentId { get; set; }
    public Plugin Parent { get; set; }
    
    [Required]
    public FileInfo FilePath { get; set; }
    
    internal static void DefineModelMetadata(ModelBuilder modelBuilder) {
        modelBuilder.Entity<PluginFileInfo>()
            .HasOne(x => x.Parent)
            .WithMany(x => x.UploadedPlugins)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<PluginFileInfo>()
            .HasIndex(x => x.ParentId);

        modelBuilder.Entity<PluginFileInfo>()
            .Property(x => x.FilePath)
            .HasConversion(x => x.FullName, x => new FileInfo(x));
    }
}