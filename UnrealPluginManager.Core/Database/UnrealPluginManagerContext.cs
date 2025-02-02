using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database.Entities.Engine;
using UnrealPluginManager.Core.Database.Entities.Plugins;

namespace UnrealPluginManager.Core.Database;

public class UnrealPluginManagerContext(DbContextOptions<UnrealPluginManagerContext> options) : DbContext(options) {
    public DbSet<Plugin> Plugins { get; init; }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        Plugin.DefineModelMetadata(modelBuilder);
        PluginAuthor.DefineModelMetadata(modelBuilder);
        Dependency.DefineModelMetadata(modelBuilder);
        EngineVersion.DefineModelMetadata(modelBuilder);
    }
}