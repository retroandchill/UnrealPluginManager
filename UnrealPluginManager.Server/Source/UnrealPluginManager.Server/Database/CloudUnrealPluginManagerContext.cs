using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Server.Config;
using UnrealPluginManager.Server.Database.Users;

namespace UnrealPluginManager.Server.Database;

/// <summary>
/// Represents the database context for the Cloud Unreal Plugin Manager, inheriting from <see cref="UnrealPluginManagerContext"/>.
/// </summary>
/// <remarks>
/// This context is specifically configured for use with SQLite as the underlying database.
/// It provides setup for database connectivity and configuration, such as batching options during database operations.
/// This context is used for managing plugin data, including uploads and metadata, within a cloud-based environment.
/// </remarks>
public class CloudUnrealPluginManagerContext : UnrealPluginManagerContext {
  private readonly PostgresConfig _config;

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

  /// <summary>
  /// Represents the specific database context for the Cloud Unreal Plugin Manager application, derived from <see cref="UnrealPluginManagerContext"/>.
  /// </summary>
  /// <remarks>
  /// Used to configure and manage database connectivity and operations in a cloud-based environment using PostgreSQL.
  /// This context handles plugin data, including plugin uploads, metadata, and related interactions.
  /// Initialization includes loading database configuration settings from an external configuration source.
  /// </remarks>
  public CloudUnrealPluginManagerContext(IConfiguration config) {
    _config = config.GetSection("Postgresql").Get<PostgresConfig>() ?? new PostgresConfig();
  }

  /// <inheritdoc />
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    optionsBuilder.UseNpgsql(_config.ConnectionString, b => b.MinBatchSize(1)
            .MaxBatchSize(100))
        .UseSnakeCaseNamingConvention();
  }

  /// <inheritdoc />
  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<User>(User.DefineModelMetadata);
    modelBuilder.Entity<ApiKey>(ApiKey.DefineModelMetadata);
    modelBuilder.Entity<PluginOwner>(PluginOwner.DefineModelMetadata);
    modelBuilder.Entity<AllowedPlugin>(AllowedPlugin.DefineModelMetadata);
  }
}