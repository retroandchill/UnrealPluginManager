using System.ComponentModel.DataAnnotations;
using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Model.Users;

/// <summary>
/// Represents the details of an API key, including its unique identifier, expiration,
/// and associated properties such as private component and allowed plugin information.
/// </summary>
public class ApiKeyDetails {

  /// <summary>
  /// Gets or sets the unique identifier for the API key.
  /// </summary>
  public required Guid Id { get; set; }
  
  public required string DisplayName { get; set; }

  /// <summary>
  /// Gets or sets the expiration date and time of the API key.
  /// </summary>
  public required DateTimeOffset ExpiresAt { get; set; }

  /// <summary>
  /// Gets or sets the private component of the API key, which is securely stored and can be used for verification purposes.
  /// </summary>
  [MaxLength(255)]
  public required string PrivateComponent { get; set; }


  /// <summary>
  /// Gets or sets the cryptographic salt used for secure hashing of the private component.
  /// </summary>
  [MaxLength(255)]
  public required string Salt { get; set; }

  /// <summary>
  /// Gets or sets the plugin glob pattern used for defining
  /// wildcard or specific plugin matching rules.
  /// </summary>
  [MaxLength(255)]
  public string? PluginGlob { get; set; }

  /// <summary>
  /// Gets or sets the list of plugin identifiers explicitly allowed for this API key.
  /// </summary>
  public List<PluginIdentifiers> AllowedPlugins { get; set; } = [];

}