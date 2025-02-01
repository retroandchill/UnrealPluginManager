using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UnrealPluginManager.Core.Model.Localization;

public class LocalizationTargetDescriptor {
    /// <summary>
    /// Name of this target
    /// </summary>
    [Required]
    [JsonPropertyName("Name")]
    public string Name { get; set; }

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