using System.IO.Abstractions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Cli.Database;

/// <summary>
/// Represents the SQLite implementation of the UnrealPluginManagerContext.
/// Provides functionality to configure and manage the database context with SQLite as the underlying database.
/// </summary>
public class SqliteUnrealPluginManagerContext(IFileSystem filesystem, IStorageService storageService) : UnrealPluginManagerContext(filesystem) {
    /// <inheritdoc />
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        Directory.CreateDirectory(storageService.BaseDirectory);
        optionsBuilder.UseSqlite($"Filename={Path.Join(storageService.BaseDirectory, "cache.sqlite")}", b =>
            b.MigrationsAssembly(Assembly.GetCallingAssembly())
                .MinBatchSize(1)
                .MaxBatchSize(100));
    }
    
    
}