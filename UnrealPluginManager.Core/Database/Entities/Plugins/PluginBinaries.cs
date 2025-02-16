using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace UnrealPluginManager.Core.Database.Entities.Plugins;

public class PluginBinaries {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }
    
    [Required]
    public ulong ParentId { get; set; }
    
    public PluginVersion Parent { get; set; }
    
    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    public required string Platform { get; set; }
    
    [Required]
    public required IFileInfo FilePath { get; set; }
    
    [Required]
    public required string EngineVersion { get; set; }
    
    internal static void DefineModelMetadata(ModelBuilder modelBuilder, IFileSystem filesystem) {
        modelBuilder.Entity<PluginBinaries>()
            .HasOne(x => x.Parent)
            .WithMany(x => x.Binaries)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PluginBinaries>()
            .HasIndex(x => x.ParentId);

        modelBuilder.Entity<PluginBinaries>()
            .Property(x => x.FilePath)
            .HasConversion(x => x.FullName, x => filesystem.FileInfo.New(x));

        modelBuilder.Entity<PluginBinaries>()
            .HasIndex(x => x.EngineVersion);
    }
}