using System.Reflection;
using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Database.Building;

namespace UnrealPluginManager.Local.Database;

/// <summary>
/// Represents the SQLite implementation of the UnrealPluginManagerContext.
/// Provides functionality to configure and manage the database context with SQLite as the underlying database.
/// </summary>
[AutoConstructor]
public partial class LocalUnrealPluginManagerContext : UnrealPluginManagerContext {
  private readonly IStorageService _storageService;

  public DbSet<PluginBuild> CachedBuilds;

  public DbSet<DependencyBuildVersion> DependencyBuildVersions;

  /// <inheritdoc />
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    Directory.CreateDirectory(_storageService.BaseDirectory);
    optionsBuilder.UseSqlite($"Filename={Path.Join(_storageService.BaseDirectory, "cache.sqlite")}", b =>
            b.MigrationsAssembly(Assembly.GetCallingAssembly())
                .MinBatchSize(1)
                .MaxBatchSize(100))
        .UseSnakeCaseNamingConvention();
  }

  /// <inheritdoc />
  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<PluginBuild>(PluginBuild.DefineMetamodelData);
    modelBuilder.Entity<DependencyBuildVersion>(DependencyBuildVersion.DefineMetamodelData);
    modelBuilder.Entity<PluginBuildPlatform>(PluginBuildPlatform.DefineMetamodelData);
  }
}