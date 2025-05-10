using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UnrealPluginManager.Core.Database.Entities.Users;

namespace UnrealPluginManager.Core.Database.Entities.Plugins;

/// <summary>
/// Represents a plugin entity within the database context.
/// </summary>
/// <remarks>
/// A plugin is a core element of the system, containing information about its identity, versioning,
/// metadata, and relationships to other plugins or files.
/// </remarks>
public class Plugin {
  /// <summary>
  /// Gets or sets the unique identifier for the plugin.
  /// </summary>
  /// <remarks>
  /// This property serves as the primary key for the Plugin entity within the database.
  /// It is an auto-generated identifier that uniquely distinguishes each plugin record.
  /// </remarks>
  [Key]
  public Guid Id { get; set; } = Guid.CreateVersion7();

  /// <summary>
  /// Gets or sets the name of the plugin.
  /// </summary>
  /// <remarks>
  /// This property represents the unique name of the plugin.
  /// The name must start with an uppercase letter and can only contain alphanumeric characters.
  /// Whitespace is not permitted within the name.
  /// </remarks>
  [Required]
  [MinLength(1)]
  [MaxLength(255)]
  [RegularExpression(@"^[A-Z][a-zA-Z0-9]+$", ErrorMessage = "Whitespace is not allowed.")]
  public required string Name { get; set; }

  /// <summary>
  /// Gets or sets the timestamp at which the plugin entity was created.
  /// </summary>
  /// <remarks>
  /// This property is automatically set to the current UTC timestamp upon initialization.
  /// It provides a record of the creation time for the plugin entity within the database.
  /// </remarks>
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

  /// <summary>
  /// Gets or sets the collection of versions associated with the plugin.
  /// </summary>
  /// <remarks>
  /// This property represents all the different versions of the plugin that have been recorded.
  /// Each version is linked to a specific plugin and managed within the context of the database.
  /// </remarks>
  public ICollection<PluginVersion> Versions { get; set; } = new List<PluginVersion>();

  /// <summary>
  /// Gets or sets the collection of users who are owners of this plugin.
  /// </summary>
  /// <remarks>
  /// This property represents a many-to-many relationship between the Plugin
  /// and User entities. Each plugin can have multiple owners, and each user
  /// can own multiple plugins. This relationship is configured in the database
  /// model metadata.
  /// </remarks>
  public ICollection<User> Owners { get; set; } = new List<User>();

  internal static void DefineModelMetadata(EntityTypeBuilder<Plugin> entity) {
    entity.HasIndex(x => new {
            x.Name
        })
        .IsUnique();
  }
}