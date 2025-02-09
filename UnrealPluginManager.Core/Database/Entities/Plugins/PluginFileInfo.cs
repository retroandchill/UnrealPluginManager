using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Abstractions;
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
    public IFileInfo FilePath { get; set; }

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