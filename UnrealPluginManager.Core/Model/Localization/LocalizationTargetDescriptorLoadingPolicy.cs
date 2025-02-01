using System.Text.Json.Serialization;

namespace UnrealPluginManager.Core.Model.Localization;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LocalizationTargetDescriptorLoadingPolicy {
    Never,
    
    Always,
    
    Editor,
    
    Game,
    
    PropertyNames,
    
    ToolTips,
}