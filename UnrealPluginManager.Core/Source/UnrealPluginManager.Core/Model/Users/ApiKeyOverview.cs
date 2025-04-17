using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;
using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Model.Users;

/// <summary>
/// Represents an overview of an API key, including its details, expiry, and associated permissions.
/// </summary>
public class ApiKeyOverview {
  /// <summary>
  /// Gets or sets the unique identifier for the API key.
  /// </summary>
  [SwaggerSchema(ReadOnly = true)]
  public Guid Id { get; set; } = Guid.CreateVersion7();

  /// <summary>
  /// Gets or sets the display name associated with the API key.
  /// </summary>
  public required string DisplayName { get; set; }

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
  public List<PluginIdentifiers> AllowedPlugins { get; set; } = [];
}