using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UnrealPluginManager.Core.Database.Entities.Engine;

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
    [MaxLength(2000)]
    public string Description { get; set; }

    [MinLength(1)]
    public ICollection<EngineVersion> CompatibleEngineVersions { get; set; }

    public ICollection<Dependency> DependsOn { get; set; }

    public ICollection<Dependency> DependedBy { get; set; }

}