using System.Text.Json.Serialization;

namespace UnrealPluginManager.Core.Model.Localization;

/// <summary>
/// Specifies the policy for loading localization targets in the Unreal Plugin Manager.
/// </summary>
/// <remarks>
/// This enumeration defines the various strategies for determining when and how a localization
/// target should be loaded. It can be used in conjunction with other components to manage
/// localization behaviors effectively within the Unreal Engine plugin ecosystem.
/// </remarks>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LocalizationTargetDescriptorLoadingPolicy {
  /// <summary>
  /// Never load.
  /// </summary>
  Never,

  /// <summary>
  /// Always load.
  /// </summary>
  Always,

  /// <summary>
  /// Only load in the editor.
  /// </summary>
  Editor,

  /// <summary>
  /// Only load in game.
  /// </summary>
  Game,

  /// <summary>
  /// Only load for property names.
  /// </summary>
  PropertyNames,

  /// <summary>
  /// Only load for tool tips.
  /// </summary>
  ToolTips,
}