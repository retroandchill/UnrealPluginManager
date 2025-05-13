using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnrealPluginManager.Core.Database.Entities.Plugins;

namespace UnrealPluginManager.Server.Database.Users;

/// <summary>
/// Represents a relationship between an API key and a plugin,
/// indicating that the plugin is allowed for the specified API key.
/// This entity defines a many-to-many relationship between API keys and plugins
/// with additional metadata to configure this relationship in the database.
/// </summary>
public class AllowedPlugin {
  /// <summary>
  /// Gets or sets the unique identifier of the API key associated with the allowed plugin.
  /// This property serves as the foreign key linking the relationship between the API key and the plugin.
  /// </summary>
  public Guid ApiKeyId { get; set; }

  /// <summary>
  /// Gets or sets the API key associated with the allowed plugin.
  /// This property establishes the relationship between the API key and the plugin, enabling authorization for the associated plugin.
  /// </summary>
  public ApiKey ApiKey { get; set; } = null!;

  /// <summary>
  /// Gets or sets the unique identifier of the associated plugin.
  /// This property serves as a foreign key linking the relationship between the plugin
  /// and the API key indicating allowed access.
  /// </summary>
  public Guid PluginId { get; set; }

  /// <summary>
  /// Gets or sets the plugin associated with the allowed plugin relationship.
  /// This property establishes the linkage to the corresponding plugin entity.
  /// </summary>
  public Plugin Plugin { get; set; } = null!;

  internal static void DefineModelMetadata(EntityTypeBuilder<AllowedPlugin> entity) {
    entity.HasKey(x => new {
        x.ApiKeyId,
        x.PluginId
    });

    entity.HasOne(x => x.ApiKey)
        .WithMany()
        .HasForeignKey(x => x.ApiKeyId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(x => x.Plugin)
        .WithMany()
        .HasForeignKey(x => x.PluginId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Cascade);
  }
}