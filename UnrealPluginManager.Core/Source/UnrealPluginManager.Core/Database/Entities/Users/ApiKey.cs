using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnrealPluginManager.Core.Database.Entities.Plugins;

namespace UnrealPluginManager.Core.Database.Entities.Users;

/// <summary>
/// Represents an API key entity used for authenticating and authorizing access to specific resources or functionalities within the system.
/// </summary>
/// <remarks>
/// The <see cref="ApiKey"/> entity is linked to a specific user and may optionally be associated with a set of plugins. Each API key includes a unique identifier, an associated user, the key itself, an expiration date, and an optional plugin glob pattern. Additionally, metadata such as relationships between users and plugins is defined for database configuration.
/// </remarks>
public class ApiKey {
  /// <summary>
  /// Gets or sets the unique identifier for the API key entity.
  /// </summary>
  /// <remarks>
  /// This identifier is a globally unique identifier (GUID) used to distinguish each API key record within the database.
  /// It is automatically generated when a new API key is created.
  /// </remarks>
  [Key]
  public Guid Id { get; set; } = Guid.CreateVersion7();

  /// <summary>
  /// Gets or sets the external identifier associated with the API key.
  /// </summary>
  /// <remarks>
  /// This identifier is used to uniquely reference the API key for external integration or lookups.
  /// It is intended for scenarios where a globally unique, client-facing identifier is required that is separate
  /// from the internal database identifier.
  /// </remarks>
  public required Guid ExternalId { get; set; }

  /// <summary>
  /// Gets or sets the display name associated with the API key.
  /// </summary>
  /// <remarks>
  /// The display name is a readable and descriptive label for the API key, typically used for identification
  /// purposes in user interfaces or logs. It is required to be specified when the API key is created.
  /// </remarks>
  [MaxLength(255)]
  public required string DisplayName { get; set; }

  /// <summary>
  /// Gets or sets the user associated with the API key.
  /// </summary>
  /// <remarks>
  /// This property establishes a relationship between the API key and its corresponding user.
  /// It serves as a navigation property, allowing access to the user entity linked to a specific API key record.
  /// The association is configured with a cascade delete rule, ensuring that when a user is deleted,
  /// their API keys are automatically removed.
  /// </remarks>
  public User User { get; set; } = null!;

  /// <summary>
  /// Gets or sets the unique identifier of the user associated with the API key.
  /// </summary>
  /// <remarks>
  /// This property represents a foreign key linking the API key to a specific user entity in the database.
  /// It is used to establish a relationship between the API key and its owning user, ensuring that each key is tied to a valid user record.
  /// </remarks>
  public Guid UserId { get; set; }

  /// <summary>
  /// Gets or sets the expiration date and time of the API key.
  /// </summary>
  /// <remarks>
  /// This property indicates the exact moment when the API key will become invalid and can no longer be used for authentication.
  /// It is specified as a <see cref="DateTimeOffset"/> to include both the date and time with a specific time zone.
  /// </remarks>
  public required DateTimeOffset ExpiresAt { get; set; }

  /// <summary>
  /// Gets or sets the glob pattern used to associate the API key with specific plugins.
  /// </summary>
  /// <remarks>
  /// This property defines a pattern that can be used to match plugin identifiers, specifying which plugins the associated API key has access to.
  /// If null or not set, the API key is not restricted by any plugin-specific limitations.
  /// </remarks>
  [MaxLength(255)]
  public string? PluginGlob { get; set; }

  /// <summary>
  /// Gets or sets the collection of plugins associated with the API key.
  /// </summary>
  /// <remarks>
  /// This collection represents the plugins that are authorized for use with the API key. The association
  /// allows permissions and access control to be scoped to specific plugins.
  /// </remarks>
  public ICollection<Plugin> Plugins { get; set; } = new List<Plugin>();

  internal static void DefineModelMetadata(EntityTypeBuilder<ApiKey> entity) {
    entity.HasOne(x => x.User)
        .WithMany(x => x.ApiKeys)
        .HasForeignKey(x => x.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasMany(x => x.Plugins)
        .WithMany()
        .UsingEntity<AllowedPlugin>();
    
    entity.Property(x => x.DisplayName)
        .HasMaxLength(255);

    entity.HasIndex(x => x.ExternalId)
        .IsUnique();

    entity.Property(x => x.PluginGlob)
        .HasMaxLength(255);
  }
}