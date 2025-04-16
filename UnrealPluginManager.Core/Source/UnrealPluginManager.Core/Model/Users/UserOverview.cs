using UnrealPluginManager.Core.Model.Storage;

namespace UnrealPluginManager.Core.Model.Users;

/// <summary>
/// Represents an overview of a User entity with essential details for identification and basic user information.
/// </summary>
public class UserOverview {
  /// <summary>
  /// Gets or sets the unique identifier associated with the user.
  /// This property is required and serves as the primary key for identifying a user.
  /// </summary>
  public required Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the username associated with the user.
  /// This property is required and uniquely identifies the user by their chosen identifier.
  /// </summary>
  public required string Username { get; set; }

  /// <summary>
  /// Gets or sets the email address associated with the user.
  /// This property is required and typically used for communication
  /// and identification purposes.
  /// </summary>
  public required string Email { get; set; }

  /// <summary>
  /// Gets or sets the profile picture associated with the user.
  /// This property contains metadata and file-related information.
  /// </summary>
  public ResourceInfo? ProfilePicture { get; set; }
}