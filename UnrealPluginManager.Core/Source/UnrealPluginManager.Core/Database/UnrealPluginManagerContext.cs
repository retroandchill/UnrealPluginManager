﻿using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Database.Entities.Storage;
using UnrealPluginManager.Core.Database.Entities.Users;

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
  /// Represents the database set for managing plugin binary data within the UnrealPluginManager context.
  /// </summary>
  /// <remarks>
  /// This property provides access to the collection of plugin binaries stored in the database.
  /// Plugin binaries include platform-specific and engine version-specific binaries associated with a plugin version.
  /// </remarks>
  public DbSet<UploadedBinaries> PluginBinaries { get; init; }

  /// <summary>
  /// Represents the database set for storing and managing file resources within the Unreal Plugin Manager context.
  /// </summary>
  /// <remarks>
  /// This property provides access to the collection of file resource entities persisted in the database.
  /// It is primarily used for operations such as adding, updating, or querying information about file resources
  /// associated with plugins or other related entities.
  /// </remarks>
  public DbSet<FileResource> FileResources { get; init; }

  /// <summary>
  /// Represents the database set for storing and managing user data within the UnrealPluginManager context.
  /// </summary>
  /// <remarks>
  /// This property provides access to the collection of users persisted in the database.
  /// It is used for performing CRUD operations and querying user-related information stored in the system.
  /// </remarks>
  public DbSet<User> Users { get; init; }

  /// <summary>
  /// Represents the database set for managing associations between plugins and their respective owners.
  /// </summary>
  /// <remarks>
  /// This property provides access to the collection of plugin ownership data, enabling CRUD operations and querying of plugin-owner relationships.
  /// It is a key component for defining and maintaining the ownership link between users and plugins within the Unreal Plugin Manager application.
  /// </remarks>
  public DbSet<PluginOwner> PluginOwners { get; init; }

  /// <summary>
  /// Represents the database set for managing API keys in the Unreal Plugin Manager context.
  /// </summary>
  /// <remarks>
  /// This property provides access to the collection of API key entities in the database.
  /// It is utilized for performing CRUD operations and validating user credentials and permissions through API keys.
  /// </remarks>
  public DbSet<ApiKey> ApiKeys { get; init; }

  /// <summary>
  /// Represents the database set for managing allowed plugins associated with API keys in the Unreal Plugin Manager system.
  /// </summary>
  /// <remarks>
  /// This property provides access to the collection of allowed plugins stored in the database.
  /// It is primarily used to associate specific plugins with API keys, enabling fine-grained access control and management.
  /// </remarks>
  public DbSet<AllowedPlugin> AllowedPlugins { get; init; }

  /// <inheritdoc/>
  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.Entity<Plugin>(Plugin.DefineModelMetadata);
    modelBuilder.Entity<PluginVersion>(PluginVersion.DefineModelMetadata);
    modelBuilder.Entity<Dependency>(Dependency.DefineModelMetadata);
    modelBuilder.Entity<UploadedBinaries>(UploadedBinaries.DefineModelMetadata);
    modelBuilder.Entity<Plugin>(Plugin.DefineModelMetadata);
    modelBuilder.Entity<User>(User.DefineModelMetadata);
    modelBuilder.Entity<ApiKey>(ApiKey.DefineModelMetadata);
    modelBuilder.Entity<PluginOwner>(PluginOwner.DefineModelMetadata);
    modelBuilder.Entity<AllowedPlugin>(AllowedPlugin.DefineModelMetadata);
  }
}