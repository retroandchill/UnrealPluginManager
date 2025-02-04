using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Semver;
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
    public string Name { get; set; } = "PlaceholderName";

    public Version Version { get; set; } = new Version(1, 0, 0);
    
    [MinLength(1)]
    [MaxLength(255)]
    public string? FriendlyName { get; set; }
    
    [MinLength(1)]
    [MaxLength(2000)]
    public string? Description { get; set; }
    
    [MinLength(1)]
    [MaxLength(255)]
    public string? AuthorName { get; set; }
    
    public Uri? AuthorWebsite { get; set; }

    public ICollection<Dependency> Dependencies { get; set; } = new List<Dependency>();

    internal static void DefineModelMetadata(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Plugin>()
            .HasIndex(x => new { x.Name, x.Version })
            .IsUnique();
        
        modelBuilder.Entity<Plugin>()
            .Property(x => x.Version)
            .HasConversion(x => x.ToString(), x => Version.Parse(x));
    }
}