using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Server.Model.Plugins;

namespace UnrealPluginManager.Server.Database.Users;

/// <summary>
/// Represents the ownership relationship between a user and a plugin.
/// </summary>
/// <remarks>
/// This class acts as a join entity between the <see cref="Users.User"/> and <see cref="Plugin"/> entities.
/// It establishes the association where a user (owner) may own one or more plugins, and a plugin may have an owner.
/// </remarks>
public class UserPlugin {
  /// <summary>
  /// Gets or sets the unique identifier of the owner (user) associated with a plugin.
  /// </summary>
  /// <remarks>
  /// This property represents the foreign key relationship to the <see cref="Users.User"/> entity,
  /// indicating which user owns the plugin. It is a required field and part of the
  /// composite key in the <see cref="UserPlugin"/> entity.
  /// </remarks>
  public Guid UserId { get; set; }

  /// <summary>
  /// Gets or sets the user who owns the associated plugin.
  /// </summary>
  /// <remarks>
  /// This property establishes the relationship to the <see cref="Users.User"/> entity,
  /// identifying the user designated as the owner of the plugin within the system.
  /// It is a required field and is part of the composite key defined in the <see cref="UserPlugin"/> entity.
  /// </remarks>
  public User User { get; set; } = null!;

  /// <summary>
  /// Gets or sets the unique identifier of the plugin associated with the ownership relationship.
  /// </summary>
  /// <remarks>
  /// This property represents the foreign key relationship to the <see cref="Plugin"/> entity,
  /// indicating the specific plugin in the ownership association. It is a required field
  /// and part of the composite key in the <see cref="UserPlugin"/> entity.
  /// </remarks>
  public Guid PluginId { get; set; }

  /// <summary>
  /// Represents a plugin entity within the system.
  /// </summary>
  /// <remarks>
  /// This entity encapsulates information about a plugin, including its unique identifier,
  /// name, description, author information, and associated metadata. It establishes relationships
  /// with other entities, such as <see cref="Users.User"/> for ownership and <see cref="PluginVersion"/>
  /// for version history. This class is a core component of the plugin management framework.
  /// </remarks>
  public Plugin Plugin { get; set; } = null!;

  /// <summary>
  /// Gets or sets the role of the user in relation to the plugin.
  /// </summary>
  /// <remarks>
  /// This property defines the user's specific role in the <see cref="UserPlugin"/> entity,
  /// indicating the level of involvement or permissions they have for the associated plugin.
  /// It is an enum of type <see cref="UserPluginRole"/>, which includes roles such as Contributor and Owner.
  /// The property is stored as a string in the database with a maximum length of 15 characters.
  /// </remarks>
  public UserPluginRole Role { get; set; }

  internal static void DefineModelMetadata(EntityTypeBuilder<UserPlugin> entity) {
    entity.HasKey(x => new {
        OwnerId = x.UserId,
        x.PluginId
    });

    entity.HasOne(x => x.User)
        .WithMany(x => x.Plugins)
        .HasForeignKey(x => x.UserId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(x => x.Plugin)
        .WithMany()
        .HasForeignKey(x => x.PluginId)
        .IsRequired()
        .OnDelete(DeleteBehavior.Cascade);

    entity.Property(x => x.Role)
        .HasConversion(x => x.ToString(), x => Enum.Parse<UserPluginRole>(x))
        .HasMaxLength(15);
  }
}