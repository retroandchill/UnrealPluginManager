using System.Reflection;
using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Local.Database;

/// <summary>
/// Represents the SQLite implementation of the UnrealPluginManagerContext.
/// Provides functionality to configure and manage the database context with SQLite as the underlying database.
/// </summary>
[AutoConstructor]
public partial class LocalUnrealPluginManagerContext : UnrealPluginManagerContext {
  private readonly IStorageService _storageService;

  /// <inheritdoc />
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    Directory.CreateDirectory(_storageService.BaseDirectory);
    optionsBuilder.UseSqlite($"Filename={Path.Join(_storageService.BaseDirectory, "cache.sqlite")}", b =>
                                 b.MigrationsAssembly(Assembly.GetCallingAssembly())
                                     .MinBatchSize(1)
                                     .MaxBatchSize(100));
  }
}