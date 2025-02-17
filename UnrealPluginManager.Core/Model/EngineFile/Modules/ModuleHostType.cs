using System.Text.Json.Serialization;

namespace UnrealPluginManager.Core.Model.Modules;

/// <summary>
/// Represents the type of host environment where a module is intended to be used in Unreal Engine.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ModuleHostType {
    /// <summary>
    /// 
    /// </summary>
    Default,

    /// <summary>
    /// Any target using the UE runtime
    /// </summary>
    Runtime,

    /// <summary>
    /// Any target except for commandlet
    /// </summary>
    RuntimeNoCommandlet,

    /// <summary>
    /// Any target or program
    /// </summary>
    RuntimeAndProgram,

    /// <summary>
    /// Loaded only in cooked builds
    /// </summary>
    CookedOnly,

    /// <summary>
    /// Loaded only in uncooked builds
    /// </summary>
    UncookedOnly,

    /// <summary>
    /// Loaded only when the engine has support for developer tools enabled
    /// </summary>
    Developer,

    /// <summary>
    /// Loads on any targets where bBuildDeveloperTools is enabled
    /// </summary>
    DeveloperTool,

    /// <summary>
    /// Loaded only by the editor
    /// </summary>
    Editor,

    /// <summary>
    /// Loaded only by the editor, except when running commandlets
    /// </summary>
    EditorNoCommandlet,

    /// <summary>
    /// Loaded by the editor or program targets
    /// </summary>
    EditorAndProgram,

    /// <summary>
    /// Loaded only by programs
    /// </summary>
    Program,

    /// <summary>
    /// Loaded only by servers
    /// </summary>
    ServerOnly,

    /// <summary>
    /// Loaded only by clients, and commandlets, and editor....
    /// </summary>
    ClientOnly,

    /// <summary>
    /// Loaded only by clients and editor (editor can run PIE which is kinda a commandlet)
    /// </summary>
    ClientOnlyNoCommandlet,

    /// <summary>
    /// External module, should never be loaded automatically only referenced
    /// </summary>
    External,
}