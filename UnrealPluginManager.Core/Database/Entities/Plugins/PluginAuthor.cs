using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnrealPluginManager.Core.Database.Entities.Plugins;

public class PluginAuthor {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }
    
    public ulong PluginId { get; set; }
    
    public Plugin Plugin { get; set; }
    
    public string AuthorName { get; set; }
    
    public Uri? AuthorWebsite { get; set; }
    
    internal static void DefineModelMetadata(ModelBuilder modelBuilder) {
        modelBuilder.Entity<PluginAuthor>()
            .HasOne(x => x.Plugin)
            .WithMany(x => x.Authors)
            .HasForeignKey(x => x.PluginId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<PluginAuthor>()
            .HasIndex(x => new { x.PluginId, x.AuthorName })
            .IsUnique();
        
        modelBuilder.Entity<PluginAuthor>()
            .HasIndex(x => new { x.AuthorName });
    }
}