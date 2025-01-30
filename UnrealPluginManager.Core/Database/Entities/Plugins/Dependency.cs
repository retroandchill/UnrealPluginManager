using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace UnrealPluginManager.Core.Database.Entities.Plugins;

public class Dependency {
    public ulong ParentId { get; set; }
    public Plugin Parent { get; set; }
    
    public ulong ChildId { get; set; }
    public Plugin Child { get; set; }

    internal static void ApplyRelationships(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Dependency>()
            .HasKey(x => new { x.ParentId, x.ChildId });
        
        modelBuilder.Entity<Dependency>()
            .HasOne(x => x.Parent)
            .WithMany(x => x.DependedBy)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Dependency>()
            .HasOne(x => x.Child)
            .WithMany(x => x.DependsOn)
            .HasForeignKey(x => x.ChildId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}