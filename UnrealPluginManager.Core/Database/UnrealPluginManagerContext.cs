using System.IO.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Semver;
using UnrealPluginManager.Core.Database.Entities.Plugins;

namespace UnrealPluginManager.Core.Database;

public class UnrealPluginManagerContext(DbContextOptions<UnrealPluginManagerContext> options) : DbContext(options) {
    public DbSet<Plugin> Plugins { get; init; }
    public DbSet<PluginFileInfo> UploadedPlugins { get; init; }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        var filesystem = Database.GetService<IFileSystem>();
        
        Plugin.DefineModelMetadata(modelBuilder);
        Dependency.DefineModelMetadata(modelBuilder);
        PluginFileInfo.DefineModelMetadata(modelBuilder, filesystem);
    }
}