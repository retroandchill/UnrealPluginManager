using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UnrealPluginManager.Core.Model.Users;

/// <summary>
/// Represents an overview of an API key, including its details, expiry, and associated permissions.
/// </summary>
public class ApiKeyOverview {
  /// <summary>
  /// Gets or sets the unique identifier for the API key.
  /// </summary>
  public required Guid Id { get; set; }

  /// <summary>
  /// Gets or sets the value of the API key.
  /// </summary>
  [JsonIgnore]
  public required string HashedKey { get; set; }

  /// <summary>
  /// Gets or sets the expiration date and time of the API key.
  /// </summary>
  public required DateTimeOffset ExpiresAt { get; set; }

  /// <summary>
  /// Gets or sets the plugin glob pattern used for defining
  /// wildcard or specific plugin matching rules.
  /// </summary>
  [MaxLength(255)]
  public string? PluginGlob { get; set; }

  /// <summary>
  /// Gets or sets the list of plugin identifiers explicitly allowed for this API key.
  /// </summary>
  public List<Guid> AllowedPlugins { get; set; } = [];
}