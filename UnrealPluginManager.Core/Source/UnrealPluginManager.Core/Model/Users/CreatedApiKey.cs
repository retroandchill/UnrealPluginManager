using System.ComponentModel;
using System.Text.Json.Serialization;
using UnrealPluginManager.Core.Converters;

namespace UnrealPluginManager.Core.Model.Users;

/// <summary>
/// Represents an API key that has been created, including its associated unique identifier,
/// key value, and expiration timestamp.
/// </summary>
public class CreatedApiKey {
  /// <summary>
  /// Gets or sets the unique identifier for the created API key.
  /// This identifier is a globally unique identifier (GUID) used to distinguish
  /// the API key from others.
  /// </summary>
  public required Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the value of the API key.
  /// This key is used for authentication and authorization purposes
  /// and acts as a secure credential enabling access to restricted resources or functionality.
  /// </summary>
  public required string ApiKey { get; set; }

  /// <summary>
  /// Gets or sets the expiration timestamp for the created API key.
  /// This value indicates when the API key will no longer be valid.
  /// </summary>
  [JsonConverter(typeof(DateTimeOffsetJsonConverter))]
  public required DateTimeOffset ExpiresOn { get; set; }
}