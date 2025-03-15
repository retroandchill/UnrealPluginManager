namespace UnrealPluginManager.Core.Model.Storage;

/// <summary>
/// Represents metadata and file-related information for a resource.
/// </summary>
/// <remarks>
/// This class is designed to encapsulate both the original file details
/// and the internally stored file attributes to manage resource data efficiently.
/// </remarks>
public class ResourceInfo {

  /// <summary>
  /// Gets or sets the unique identifier for the resource.
  /// </summary>
  /// <remarks>
  /// This property represents a globally unique identifier (GUID) used to uniquely identify
  /// the resource in the system. It serves as the primary key for the resource.
  /// </remarks>
  public Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the original filename of the resource before any processing or storage modifications.
  /// </summary>
  /// <remarks>
  /// This property represents the name of the file in its original form as it was before being imported or managed by the system.
  /// Used for tracking the source file's metadata and ensuring traceability.
  /// </remarks>
  public required string OriginalFilename { get; set; }

  /// <summary>
  /// Gets the name of the file as stored in the underlying file system.
  /// </summary>
  /// <remarks>
  /// This property retrieves the name of the file that is stored internally
  /// within the application. It reflects the actual name used in the storage
  /// system, which may differ from the original filename provided by the user.
  /// </remarks>
  public string StoredFilename { get; set; }

}