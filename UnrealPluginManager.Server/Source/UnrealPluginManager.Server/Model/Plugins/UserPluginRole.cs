using System.Text.Json.Serialization;

namespace UnrealPluginManager.Server.Model.Plugins;

/// <summary>
/// Defines the roles that a user can have in relation to a plugin.
/// </summary>
/// <remarks>
/// The enum is serialized and deserialized as strings for storage and communication purposes.
/// </remarks>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UserPluginRole {
  /// <summary>
  /// A user contributing to the plugin without full ownership.
  /// </summary>
  Contributor,

  /// <summary>
  /// The primary user responsible for the plugin.
  /// </summary>
  Owner
}