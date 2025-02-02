using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database.Entities.Engine;
using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Database.Entities.Plugins;

public class Plugin {
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public ulong Id { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(255)]
    [RegularExpression(@"^[A-Z][a-zA-Z0-9]+$", ErrorMessage = "Whitespace is not allowed.")]
    public string Name { get; set; }
    
    [MinLength(1)]
    [MaxLength(255)]
    public string? FriendlyName { get; set; }
    
    [MinLength(1)]
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    public ICollection<PluginAuthor> Authors { get; set; } = new List<PluginAuthor>();

    public PluginType Type { get; set; } = PluginType.Provided;

    [MinLength(1)]
    public ICollection<EngineVersion> CompatibleEngineVersions { get; set; } = new List<EngineVersion>();

    public ICollection<Dependency> DependsOn { get; set; } = new List<Dependency>();

    public ICollection<Dependency> DependedBy { get; set; } = new List<Dependency>();

    internal static void DefineModelMetadata(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Plugin>()
            .HasIndex(x => x.Name)
            .IsUnique();
        
        modelBuilder.Entity<Plugin>()
            .HasIndex(x => x.Type);
    }
}