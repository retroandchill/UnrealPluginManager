using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Semver;
using UnrealPluginManager.Core.Database.Entities.Plugins;

namespace UnrealPluginManager.Core.Database;

public class UnrealPluginManagerContext(DbContextOptions<UnrealPluginManagerContext> options) : DbContext(options) {
    public DbSet<Plugin> Plugins { get; init; }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        Plugin.DefineModelMetadata(modelBuilder);
        Dependency.DefineModelMetadata(modelBuilder);
        PluginFileInfo.DefineModelMetadata(modelBuilder);
    }
}