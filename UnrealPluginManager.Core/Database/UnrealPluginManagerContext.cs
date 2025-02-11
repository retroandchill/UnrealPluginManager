using System.IO.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Semver;
using UnrealPluginManager.Core.Database.Entities.Plugins;

namespace UnrealPluginManager.Core.Database;

/// <summary>
/// Represents the entity framework database context for managing plugin data and related entities in the Unreal Plugin Manager application.
/// </summary>
/// <remarks>
/// This context is used for interacting with plugin information and associated data stored in the database.
/// It leverages the configuration provided by the <see cref="DbContextOptions{TContext}"/> to establish the connection and behavior for the database.
/// </remarks>
public abstract class UnrealPluginManagerContext(IFileSystem filesystem) : DbContext {
    /// <summary>
    /// Represents the database set for storing and managing plugin data within the UnrealPluginManager context.
    /// </summary>
    /// <remarks>
    /// This property provides access to the collection of plugins persisted in the database.
    /// It is used for performing CRUD operations and querying the plugins stored in the system.
    /// </remarks>
    public DbSet<Plugin> Plugins { get; init; }

    /// <summary>
    /// Represents the database set for storing and managing uploaded plugin file metadata within the UnrealPluginManager context.
    /// </summary>
    /// <remarks>
    /// This property provides access to the collection of PluginFileInfo entities that detail the file paths, engine versions, and parent plugin references for uploaded plugins.
    /// It is utilized for querying, storing, and updating metadata about plugin files associated with the system.
    /// </remarks>
    public DbSet<PluginFileInfo> UploadedPlugins { get; init; }

    /// <inheritdoc/>
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        Plugin.DefineModelMetadata(modelBuilder);
        Dependency.DefineModelMetadata(modelBuilder);
        PluginFileInfo.DefineModelMetadata(modelBuilder, filesystem);
    }
}