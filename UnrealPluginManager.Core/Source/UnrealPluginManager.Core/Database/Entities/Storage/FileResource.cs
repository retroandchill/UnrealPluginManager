using System.ComponentModel.DataAnnotations;

namespace UnrealPluginManager.Core.Database.Entities.Storage;

/// <summary>
/// Represents a file resource entity used within the database context.
/// </summary>
/// <remarks>
/// This class is used to manage information about stored files, including their original filename,
/// creation timestamp, and file system path. It also includes methods to define entity model metadata.
/// </remarks>
public class FileResource {

  /// <summary>
  /// Gets or sets the unique identifier for the file resource entity.
  /// </summary>
  /// <remarks>
  /// This property represents the primary key of the FileResource entity, which is used to uniquely identify a file within the database.
  /// The identifier is generated using Version 7 UUIDs to ensure uniqueness and scalability.
  /// </remarks>
  [Key]
  public Guid Id { get; set; } = Guid.CreateVersion7();

  /// <summary>
  /// Gets or sets the original filename of the file resource as provided during upload or creation.
  /// </summary>
  /// <remarks>
  /// This property identifies the name of the file as originally provided by the user or source system.
  /// It may differ from the final stored filename or path within the storage system.
  /// The value is limited to a maximum length of 255 characters.
  /// </remarks>
  [MaxLength(255)]
  public required string OriginalFilename { get; set; }

  /// <summary>
  /// Gets or sets the timestamp indicating when the file resource entity was created.
  /// </summary>
  /// <remarks>
  /// This property captures the creation date and time of the file resource entity.
  /// The value is recorded in UTC format using DateTimeOffset, ensuring accuracy
  /// and consistency across different time zones in the system.
  /// </remarks>
  public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

  /// <summary>
  /// Gets or sets the name of the file as stored in the file system.
  /// </summary>
  /// <remarks>
  /// This property represents the file's actual name on the storage medium, which may differ from its original name.
  /// It is used to ensure that the file is uniquely identifiable within the storage location.
  /// The filename is subject to a maximum length constraint of 255 characters.
  /// </remarks>
  [MaxLength(255)]
  public required string StoredFilename { get; set; }

}