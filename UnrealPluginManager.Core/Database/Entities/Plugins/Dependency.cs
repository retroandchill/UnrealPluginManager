using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Semver;
using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Database.Entities.Plugins;

public class Dependency {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }
    
    public ulong ParentId { get; set; }
    public Plugin Parent { get; set; }
    
    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    [RegularExpression(@"^[A-Z][a-zA-Z0-9]+$", ErrorMessage = "Whitespace is not allowed.")]
    public string PluginName { get; set; }
    
    [MinLength(1)]
    [MaxLength(255)]
    public SemVersionRange PluginVersion { get; set; } = SemVersionRange.All;
    
    public bool Optional { get; set; }
    
    public PluginType Type { get; set; } = PluginType.Provided;

    internal static void DefineModelMetadata(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Dependency>()
            .HasOne(x => x.Parent)
            .WithMany(x => x.Dependencies)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Dependency>()
            .HasIndex(x => x.ParentId);
        
        modelBuilder.Entity<Dependency>()
            .Property(x => x.PluginVersion)
            .HasConversion(
                x => x.ToString(), 
                x => SemVersionRange.Parse(x, 2048));
    }
}