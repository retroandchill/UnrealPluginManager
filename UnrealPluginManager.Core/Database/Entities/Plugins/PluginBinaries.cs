using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace UnrealPluginManager.Core.Database.Entities.Plugins;

public class PluginBinaries {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }
    
    public ulong ParentId { get; set; }
    
    public PluginVersion Parent { get; set; }
    
    [MinLength(1)]
    [MaxLength(255)]
    public required string Platform { get; set; }
    
    public required IFileInfo FilePath { get; set; }
    
    public Version EngineVersion { get; set; } = new(4, 27, 0);
    
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

        modelBuilder.Entity<PluginBinaries>()
            .Property(x => x.EngineVersion)
            .HasConversion(x => x.ToString(), x => Version.Parse(x));
    }
}