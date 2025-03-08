using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using UnrealPluginManager.Core.Meta;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Targets;

namespace UnrealPluginManager.Core.Model.Modules;

/// <summary>
/// Represents a descriptor for an Unreal Engine module, containing metadata
/// and configuration options that define its behavior and compatibility.
/// </summary>
public class ModuleDescriptor {
  /// <summary>
  /// Name of this module
  /// </summary>
  [Required]
  [JsonPropertyName("Name")]
  public required string Name { get; set; }

  /// <summary>
  /// Usage type of module
  /// </summary>
  [Required]
  [JsonPropertyName("Type")]
  public ModuleHostType Type { get; set; }

  /// <summary>
  /// When should the module be loaded during the startup sequence?  This is sort of an advanced setting.
  /// </summary>
  [JsonPropertyName("LoadingPhase")]
  [DefaultValue(ModuleLoadingPhase.Default)]
  public ModuleLoadingPhase LoadingPhase { get; set; } = ModuleLoadingPhase.Default;

  /// <summary>
  /// List of allowed platforms
  /// </summary>
  [JsonPropertyName("PlatformAllowList")]
  [DefaultAsEmpty]
  public List<string> PlatformAllowList { get; set; } = [];

  /// <summary>
  /// List of disallowed platforms
  /// </summary>
  [JsonPropertyName("PlatformDenyList")]
  [DefaultAsEmpty]
  public List<string> PlatformDenyList { get; set; } = [];

  /// <summary>
  /// List of allowed targets
  /// </summary>
  [JsonPropertyName("TargetAllowList")]
  [DefaultAsEmpty]
  public List<TargetType> TargetAllowList { get; set; } = [];

  /// <summary>
  /// List of disallowed targets
  /// </summary>
  [JsonPropertyName("TargetDenyList")]
  [DefaultAsEmpty]
  public List<TargetType> TargetDenyList { get; set; } = [];

  /// <summary>
  /// List of allowed target configurations
  /// </summary>
  [JsonPropertyName("TargetConfigurationAllowList")]
  [DefaultAsEmpty]
  public List<UnrealTargetConfiguration> TargetConfigurationAllowList { get; set; } = [];

  /// <summary>
  /// List of disallowed target configurations
  /// </summary>
  [JsonPropertyName("TargetConfigurationDenyList")]
  [DefaultAsEmpty]
  public List<UnrealTargetConfiguration> TargetConfigurationDenyList { get; set; } = [];

  /// <summary>
  /// List of allowed programs
  /// </summary>
  [JsonPropertyName("ProgramAllowList")]
  [DefaultAsEmpty]
  public List<string> ProgramAllowList { get; set; } = [];

  /// <summary>
  /// List of disallowed programs
  /// </summary>
  [JsonPropertyName("ProgramDenyList")]
  [DefaultAsEmpty]
  public List<string> ProgramDenyList { get; set; } = [];

  /// <summary>
  /// List of allowed game targets
  /// </summary>
  [JsonPropertyName("GameTargetAllowList")]
  [DefaultAsEmpty]
  public List<string> GameTargetAllowList { get; set; } = [];

  /// <summary>
  /// List of disallowed game targets
  /// </summary>
  [JsonPropertyName("GameTargetDenyList")]
  [DefaultAsEmpty]
  public List<string> GameTargetDenyList { get; set; } = [];

  /// <summary>
  /// List of additional dependencies for building this module.
  /// </summary>
  [JsonPropertyName("AdditionalDependencies")]
  [DefaultAsEmpty]
  public List<string> AdditionalDependencies { get; set; } = [];

  /// <summary>
  /// When true, an empty PlatformAllowList is interpreted as 'no platforms' with the expectation that explicit platforms will be added in plugin extensions */
  /// </summary>
  [JsonPropertyName("bHasExplicitPlatforms")]
  [DefaultValue(false)]
  public bool HasExplicitPlatforms { get; set; }
}