using System.Text.Json.Serialization;

namespace UnrealPluginManager.Core.Model.Plugins;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PluginType {
    Engine,
    Provided,
    External
}