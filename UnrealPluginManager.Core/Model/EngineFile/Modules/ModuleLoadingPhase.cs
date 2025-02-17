using System.Text.Json.Serialization;

namespace UnrealPluginManager.Core.Model.Modules;

/// <summary>
/// Represents the phase during which a module is loaded within the startup sequence.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ModuleLoadingPhase {
    /// <summary>
    /// Loaded at the default loading point during startup (during engine init, after game modules are loaded.)
    /// </summary>
    Default,

    /// <summary>
    /// Right after the default phase
    /// </summary>
    PostDefault,

    /// <summary>
    /// Right before the default phase
    /// </summary>
    PreDefault,

    /// <summary>
    /// Loaded as soon as plugins can possibly be loaded (need GConfig)
    /// </summary>
    EarliestPossible,

    /// <summary>
    /// Loaded before the engine is fully initialized, immediately after the config system has been initialized.  Necessary only for very low-level hooks
    /// </summary>
    PostConfigInit,

    /// <summary>
    /// The first screen to be rendered after system splash screen
    /// </summary>
    PostSplashScreen,

    /// <summary>
    /// After PostConfigInit and before coreUobject initialized. used for early boot loading screens before the uobjects are initialized
    /// </summary>
    PreEarlyLoadingScreen,

    /// <summary>
    /// Loaded before the engine is fully initialized for modules that need to hook into the loading screen before it triggers
    /// </summary>
    PreLoadingScreen,

    /// <summary>
    /// After the engine has been initialized
    /// </summary>
    PostEngineInit,

    /// <summary>
    /// Do not automatically load this module
    /// </summary>
    None,
}