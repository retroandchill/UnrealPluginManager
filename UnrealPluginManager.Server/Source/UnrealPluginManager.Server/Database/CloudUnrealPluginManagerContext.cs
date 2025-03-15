using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Server.Config;

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
  /// Represents the specific database context for the Cloud Unreal Plugin Manager application, derived from <see cref="UnrealPluginManagerContext"/>.
  /// </summary>
  /// <remarks>
  /// Used to configure and manage database connectivity and operations in a cloud-based environment using PostgreSQL.
  /// This context handles plugin data, including plugin uploads, metadata, and related interactions.
  /// Initialization includes loading database configuration settings from an external configuration source.
  /// </remarks>
  public CloudUnrealPluginManagerContext(IConfiguration config) {
    _config = new PostgresConfig();
    config.GetSection("Postgresql").Bind(_config);
  }

  /// <inheritdoc />
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
    optionsBuilder.UseNpgsql(_config.ConnectionString, b => b.MinBatchSize(1)
            .MaxBatchSize(100))
        .UseSnakeCaseNamingConvention();
  }
}