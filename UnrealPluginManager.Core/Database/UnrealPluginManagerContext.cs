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
public abstract class UnrealPluginManagerContext : DbContext {
  /// <summary>
  /// Represents the entity framework database context for managing plugin data and related entities in the Unreal Plugin Manager application.
  /// </summary>
  /// <remarks>
  /// This abstract class serves as the base context for interacting with plugin-related data across multiple implementations.
  /// It leverages the provided <see cref="IFileSystem"/> instance to handle file-related operations relevant to the database and application logic.
  /// </remarks>
  protected UnrealPluginManagerContext(IFileSystem filesystem) {
  }

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

  /// <inheritdoc/>
  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    Plugin.DefineModelMetadata(modelBuilder);
    PluginVersion.DefineModelMetadata(modelBuilder);
    Dependency.DefineModelMetadata(modelBuilder);
    UploadedBinaries.DefineModelMetadata(modelBuilder);
  }
}