﻿using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Database.Entities.Storage;

namespace UnrealPluginManager.Core.Database;

/// <summary>
/// Represents the entity framework database context for managing plugin data and related entities in the Unreal Plugin Manager application.
/// </summary>
/// <remarks>
/// This context is used for interacting with plugin information and associated data stored in the database.
/// It leverages the configuration provided by the <see cref="DbContextOptions{TContext}"/> to establish the connection and behavior for the database.
/// </remarks>
public abstract class UnrealPluginManagerContext : DbContext {

  /// <summary>
  /// Represents the database set for storing and managing plugin data within the UnrealPluginManager context.
  /// </summary>
  /// <remarks>
  /// This property provides access to the collection of plugins persisted in the database.
  /// It is used for performing CRUD operations and querying the plugins stored in the system.
  /// </remarks>
  public DbSet<Plugin> Plugins { get; init; }

  /// <summary>
  /// Represents the database set for storing and managing versions of plugins within the UnrealPluginManager context.
  /// </summary>
  /// <remarks>
  /// This property provides access to the collection of plugin versions persisted in the database.
  /// It is primarily used for handling operations related to specific versions of plugins, such as querying, updating, or adding new versions.
  /// </remarks>
  public DbSet<PluginVersion> PluginVersions { get; init; }

  /// <summary>
  /// Represents the database set for storing and managing patches associated with specific plugin versions within the UnrealPluginManager context.
  /// </summary>
  /// <remarks>
  /// This property provides access to the collection of `PluginSourcePatch` entities persisted in the database.
  /// It is used for performing CRUD operations and querying the patch data related to plugin versions.
  /// </remarks>
  public DbSet<PluginSourcePatch> PluginSourcePatches { get; init; }

  /// <summary>
  /// Represents the database set for storing and managing file resources within the Unreal Plugin Manager context.
  /// </summary>
  /// <remarks>
  /// This property provides access to the collection of file resource entities persisted in the database.
  /// It is primarily used for operations such as adding, updating, or querying information about file resources
  /// associated with plugins or other related entities.
  /// </remarks>
  public DbSet<FileResource> FileResources { get; init; }

  /// <inheritdoc/>
  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.Entity<Plugin>(Plugin.DefineModelMetadata);
    modelBuilder.Entity<PluginVersion>(PluginVersion.DefineModelMetadata);
    modelBuilder.Entity<Dependency>(Dependency.DefineModelMetadata);
    modelBuilder.Entity<PluginSourcePatch>(PluginSourcePatch.DefineModelMetadata);
  }
}