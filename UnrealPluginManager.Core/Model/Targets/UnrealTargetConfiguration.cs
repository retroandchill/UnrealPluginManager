using System.Text.Json.Serialization;

namespace UnrealPluginManager.Core.Model.Targets;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum UnrealTargetConfiguration {
    /// <summary>
    /// Unknown
    /// </summary>
    Unknown,

    /// <summary>
    /// Debug configuration
    /// </summary>
    Debug,

    /// <summary>
    /// DebugGame configuration; equivalent to development, but with optimization disabled for game modules
    /// </summary>
    DebugGame,

    /// <summary>
    /// Development configuration
    /// </summary>
    Development,

    /// <summary>
    /// Test configuration
    /// </summary>
    Test,

    /// <summary>
    /// Shipping configuration
    /// </summary>
    Shipping,
}