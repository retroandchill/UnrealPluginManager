using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Database.Entities.Storage;

namespace UnrealPluginManager.Core.Database.Entities.Users;

/// <summary>
/// Represents a user entity within the database.
/// </summary>
/// <remarks>
/// This class defines the properties and relationships of a user in the
/// Unreal Plugin Manager system. Each user is uniquely identified by their
/// <see cref="Id"/> and is associated with a collection of plugins they own
/// or manage. Users can optionally have a profile picture represented by a
/// <see cref="FileResource"/>.
/// </remarks>
public class User {
  /// <summary>
  /// Gets or sets the unique identifier for the user entity.
  /// </summary>
  /// <remarks>
  /// Each user in the database is uniquely identified by this property. The identifier
  /// is automatically generated as a version-7 GUID to ensure uniqueness and meet the
  /// database constraints. It acts as the primary key for the <see cref="User"/> entity.
  /// </remarks>
  [Key]
  public Guid Id { get; set; } = Guid.CreateVersion7();

  /// <summary>
  /// Gets or sets the username for the user.
  /// </summary>
  /// <remarks>
  /// This property represents the username chosen by the user for their account within
  /// the system. It is used as an identifier for authentication and user-related operations
  /// such as login and profile display.
  /// </remarks>
  [MaxLength(31)]
  public required string Username { get; set; }

  /// <summary>
  /// Gets or sets the hashed representation of the user's password.
  /// </summary>
  /// <remarks>
  /// This property stores the securely hashed and salted version of the user's password.
  /// It is used for authentication and is never stored in plaintext.
  /// The maximum length is constrained to 32 characters, adhering to database design constraints.
  /// </remarks>
  [MaxLength(31)]
  public string? Password { get; set; }

  /// <summary>
  /// Gets or sets the email address associated with the user.
  /// </summary>
  /// <remarks>
  /// This property holds the user's email address, which serves as a primary communication
  /// contact. The value must comply with standard email format conventions and may be used
  /// for actions such as login, notifications, or account recovery.
  /// </remarks>
  [MaxLength(255)]
  [StringSyntax(StringSyntaxAttribute.Uri)]
  public required string Email { get; set; }

  /// <summary>
  /// Gets or sets the profile picture associated with the user.
  /// </summary>
  /// <remarks>
  /// This property references an optional <see cref="FileResource"/> object that represents
  /// the user's profile picture. The associated file is stored in the database, and its details
  /// are managed through the <see cref="FileResource"/> entity. If no profile picture is provided,
  /// this property can remain null.
  /// </remarks>
  public FileResource? ProfilePicture { get; set; }

  /// <summary>
  /// Gets or sets the unique identifier for the associated profile picture file resource.
  /// </summary>
  /// <remarks>
  /// This property references the <see cref="FileResource.Id"/> of the profile picture
  /// associated with the user. It can be null if no profile picture is assigned. The
  /// relationship between the user and the file resource is established via this identifier.
  /// </remarks>
  public Guid? ProfilePictureId { get; set; }

  /// <summary>
  /// Gets or sets the collection of plugins associated with the user.
  /// </summary>
  /// <remarks>
  /// This property represents a many-to-many relationship between users and plugins. It defines
  /// the plugins that the user owns or manages. Each plugin in the collection is associated
  /// with its respective metadata and ownership structure within the system.
  /// </remarks>
  public ICollection<Plugin> Plugins { get; set; } = new List<Plugin>();

  /// <summary>
  /// Gets or sets the collection of API keys associated with the user.
  /// </summary>
  /// <remarks>
  /// This property represents the API keys that belong to the user. Each API key is tied
  /// to a specific user and can be used to authenticate or authorize actions within the
  /// Unreal Plugin Manager system. The collection is maintained as a one-to-many
  /// relationship between the <see cref="User"/> and <see cref="ApiKey"/> entities.
  /// </remarks>
  public ICollection<ApiKey> ApiKeys { get; set; } = new List<ApiKey>();

  internal static void DefineModelMetadata(EntityTypeBuilder<User> entity) {
    entity.HasMany(x => x.Plugins)
        .WithMany(x => x.Owners)
        .UsingEntity<PluginOwner>();

    entity.Property(x => x.Username)
        .HasMaxLength(31);

    entity.Property(x => x.Password)
        .HasMaxLength(31);

    entity.Property(x => x.Email)
        .HasMaxLength(255);

    entity.HasOne(x => x.ProfilePicture)
        .WithMany()
        .HasForeignKey(x => x.ProfilePictureId)
        .OnDelete(DeleteBehavior.NoAction);
  }
}