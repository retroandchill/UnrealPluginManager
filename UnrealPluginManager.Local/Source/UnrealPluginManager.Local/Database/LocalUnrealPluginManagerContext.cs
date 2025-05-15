using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Retro.ReadOnlyParams.Annotations;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Local.Database.Building;

namespace UnrealPluginManager.Local.Database;

/// <summary>
/// Represents the SQLite implementation of the UnrealPluginManagerContext.
/// Provides functionality to configure and manage the database context with SQLite as the underlying database.
/// </summary>
public class LocalUnrealPluginManagerContext([ReadOnly] IStorageService storageService) : UnrealPluginManagerContext {
  /// <summary>
  /// Represents a collection of cached plugin builds within the database.
  /// This property is a DbSet of <see cref="PluginBuild"/> entities, allowing the management
  /// of build data such as storing, querying, and deleting cached builds.
  /// </summary>
  public DbSet<PluginBuild> CachedBuilds { get; set; }

  /// <summary>
  /// Represents a collection of dependency build versions within the database.
  /// This property is a DbSet of <see cref="DependencyBuildVersion"/> entities, enabling the management
  /// of build version data related to plugin dependencies, such as storing, querying, and deleting entries.
  /// </summary>
  public DbSet<DependencyBuildVersion> DependencyBuildVersions { get; set; }

  /// <summary>
  /// Represents a collection of plugin build platforms within the database.
  /// This property is a DbSet of <see cref="PluginBuildPlatform"/> entities, enabling the management
  /// of platform-specific data for plugin builds, such as storing, querying, and deleting platform records.
  /// </summary>
  public DbSet<PluginBuildPlatform> PluginBuildPlatforms { get; set; }

  /// <inheritdoc />
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    Directory.CreateDirectory(storageService.BaseDirectory);
    optionsBuilder.UseSqlite($"Filename={Path.Join(storageService.BaseDirectory, "cache.sqlite")}", b =>
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