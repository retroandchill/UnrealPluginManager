using System.Text.Json.Serialization;

namespace UnrealPluginManager.Core.Model.Localization;

/// <summary>
/// Represents the policy for generating localization configuration files for a localization target.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LocalizationConfigGenerationPolicy {
  /// <summary>
  /// This localization target should never have localization config files associated with it during the localization gather pipeline.
  /// </summary>
  Never,

  /// <summary>
  /// This localization target should only use user generated localization config files during the localization gather pipeline.
  /// </summary>
  User,

  /// <summary>
  /// Default auto-generated localization config files will be used to generate the localization target and localization content files during the localization gather pipeline
  /// </summary>
  Auto,
}