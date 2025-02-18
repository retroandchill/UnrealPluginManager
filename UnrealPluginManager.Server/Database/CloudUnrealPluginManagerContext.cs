using System.IO.Abstractions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;

namespace UnrealPluginManager.Server.Database;

/// <summary>
/// Represents the database context for the Cloud Unreal Plugin Manager, inheriting from <see cref="UnrealPluginManagerContext"/>.
/// </summary>
/// <remarks>
/// This context is specifically configured for use with SQLite as the underlying database.
/// It provides setup for database connectivity and configuration, such as batching options during database operations.
/// This context is used for managing plugin data, including uploads and metadata, within a cloud-based environment.
/// </remarks>
public class CloudUnrealPluginManagerContext(IFileSystem filesystem) : UnrealPluginManagerContext(filesystem) {

    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseSqlite("Filename=dev.sqlite", b =>
            b.MinBatchSize(1)
                .MaxBatchSize(100));
    }
    
}