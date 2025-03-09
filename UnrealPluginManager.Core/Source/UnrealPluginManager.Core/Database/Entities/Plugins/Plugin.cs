using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Semver;
using UnrealPluginManager.Core.Model.Plugins;

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
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public long Id { get; set; }

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
  /// Gets or sets the user-friendly, display name of the plugin.
  /// </summary>
  /// <remarks>
  /// This property represents an optional, human-readable identifier
  /// that can be used to display or describe the plugin in a more
  /// descriptive and accessible manner compared to its technical name.
  /// It supports up to 255 characters and adheres to defined length constraints.
  /// </remarks>
  [MinLength(1)]
  [MaxLength(255)]
  public string? FriendlyName { get; set; }

  /// <summary>
  /// Gets or sets the description of the plugin.
  /// </summary>
  /// <remarks>
  /// This property contains a textual representation that describes the plugin's purpose, functionality, or other relevant details.
  /// It is constrained to a string length between 1 and 2000 characters.
  /// </remarks>
  [MinLength(1)]
  [MaxLength(2000)]
  public string? Description { get; set; }

  /// <summary>
  /// Gets or sets the name of the author of the plugin.
  /// </summary>
  /// <remarks>
  /// This property contains the name of the individual or entity responsible for creating the plugin.
  /// It is optional but must adhere to the specified length and format restrictions if provided.
  /// </remarks>
  [MinLength(1)]
  [MaxLength(255)]
  public string? AuthorName { get; set; }

  /// <summary>
  /// Gets or sets the website of the plugin's author.
  /// </summary>
  /// <remarks>
  /// The website provides additional information about the author, such as their portfolio,
  /// project details, or other plugins they have developed. It may be null if the author
  /// has not provided a website.
  /// </remarks>
  [MaxLength(255)]
  public Uri? AuthorWebsite { get; set; }

  /// <summary>
  /// Gets or sets the collection of versions associated with the plugin.
  /// </summary>
  /// <remarks>
  /// This property represents all the different versions of the plugin that have been recorded.
  /// Each version is linked to a specific plugin and managed within the context of the database.
  /// </remarks>
  public ICollection<PluginVersion> Versions { get; set; } = new List<PluginVersion>();

  internal static void DefineModelMetadata(ModelBuilder modelBuilder) {
    modelBuilder.Entity<Plugin>()
        .HasIndex(x => new { x.Name })
        .IsUnique();
  }
}