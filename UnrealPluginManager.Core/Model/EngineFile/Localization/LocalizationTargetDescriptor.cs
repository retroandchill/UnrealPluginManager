using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UnrealPluginManager.Core.Model.Localization;

/// <summary>
/// Represents the configuration details of a localization target within the Unreal Plugin Manager.
/// </summary>
/// <remarks>
/// This class encapsulates essential properties and metadata required to define and manage
/// a localization target. It includes the name of the target, loading policies, and configuration
/// generation policies for handling localization features in Unreal Engine plugin projects.
/// </remarks>
/// <seealso cref="UnrealPluginManager.Core.Model.Localization.LocalizationTargetDescriptorLoadingPolicy" />
/// <seealso cref="UnrealPluginManager.Core.Model.Localization.LocalizationConfigGenerationPolicy" />
public class LocalizationTargetDescriptor {
    /// <summary>
    /// Name of this target
    /// </summary>
    [Required]
    [JsonPropertyName("Name")]
    public required string Name { get; set; }

    /// <summary>
    /// When should the localization data associated with a target should be loaded?
    /// </summary>
    [Required]
    [JsonPropertyName("LoadingPolicy")]
    public LocalizationTargetDescriptorLoadingPolicy LoadingPolicy { get; set; }

    /// <summary>
    /// How should this localization target's localization config files be generated during a localization gather.
    /// </summary>
    [Required]
    [JsonPropertyName("ConfigGenerationPolicy")]
    public LocalizationConfigGenerationPolicy ConfigGenerationPolicy { get; set; }
}