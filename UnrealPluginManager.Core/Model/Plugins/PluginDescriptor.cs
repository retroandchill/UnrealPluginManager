using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Semver;
using UnrealPluginManager.Core.Converters;
using UnrealPluginManager.Core.Meta;
using UnrealPluginManager.Core.Model.Localization;
using UnrealPluginManager.Core.Model.Modules;
using UnrealPluginManager.Core.Model.Scripting;

namespace UnrealPluginManager.Core.Model.Plugins;

using CustomBuildSteps = Dictionary<string, List<string>>;

/// <summary>
/// Represents the metadata and configuration for an Unreal Engine plugin.
/// This class holds various attributes describing the plugin, including its version, creator information,
/// supported platforms, dependencies, and additional settings related to content, extensions, and build configuration.
/// </summary>
public class PluginDescriptor {
    /// <summary>
    /// Descriptor version number
    /// </summary>
    [Required]
    [JsonPropertyName("FileVersion")]
    public int FileVersion { get; set; } = 3;

    /// <summary>
    /// Version number for the plugin.  The version number must increase with every version of the plugin, so that the system 
    /// can determine whether one version of a plugin is newer than another, or to enforce other requirements.  This version
    /// number is not displayed in front-facing UI.  Use the VersionName for that.
    /// </summary>
    [Required]
    [JsonPropertyName("Version")]
    public int Version { get; set; }

    /// <summary>
    /// Name of the version for this plugin.  This is the front-facing part of the version number.  It doesn't need to match
    /// the version number numerically, but should be updated when the version number is increased accordingly.
    /// </summary>
    [Required]
    [JsonPropertyName("VersionName")]
    [JsonConverter(typeof(SemVersionJsonConverter))]
    public SemVersion VersionName { get; set; } = new(1, 0, 0);

    /// <summary>
    /// Friendly name of the plugin
    /// </summary>
    [JsonPropertyName("FriendlyName")]
    public string? FriendlyName { get; set; }

    /// <summary>
    /// Description of the plugin
    /// </summary>
    [JsonPropertyName("Description")]
    public string? Description { get; set; }

    /// <summary>
    /// The name of the category this plugin
    /// </summary>
    [JsonPropertyName("Category")]
    public string? Category { get; set; }

    /// <summary>
    /// The company or individual who created this plugin.  This is an optional field that may be displayed in the user interface.
    /// </summary>
    [JsonPropertyName("CreatedBy")]
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Hyperlink URL string for the company or individual who created this plugin.  This is optional.
    /// </summary>
    [JsonPropertyName("CreatedByURL")]
    public Uri? CreatedByUrl { get; set; }

    /// <summary>
    /// Documentation URL string.
    /// </summary>
    [JsonPropertyName("DocsURL")]
    public Uri? DocsUrl { get; set; }

    /// <summary>
    /// Marketplace URL for this plugin. This URL will be embedded into projects that enable this plugin, so we can redirect to the marketplace if a user doesn't have it installed.
    /// </summary>
    [JsonPropertyName("MarketplaceURL")]
    public Uri? MarketplaceUrl { get; set; }

    /// <summary>
    /// Support URL/email for this plugin.
    /// </summary>
    [JsonPropertyName("SupportURL")]
    public Uri? SupportUrl { get; set; }

    /// <summary>
    /// Sets the version of the engine that this plugin is compatible with.
    /// </summary>
    [JsonPropertyName("EngineVersion")]
    public Version? EngineVersion { get; set; }

    /// <summary>
    /// Sets the version of the engine at which this plugin has been deprecated.
    /// </summary>
    [JsonPropertyName("DeprecatedEngineVersion")]
    public Version? DeprecatedEngineVersion { get; set; }

    /// <summary>
    /// If true, this plugin from a platform extension extending another plugin
    /// </summary>
    [JsonPropertyName("bIsPluginExtension")]
    public bool IsPluginExtension { get; set; } = false;

    /// <summary>
    /// List of platforms supported by this plugin. This list will be copied to any plugin reference from a project file, to allow filtering entire plugins from staged builds.
    /// </summary>
    [JsonPropertyName("SupportedTargetPlatforms")]
    [DefaultAsEmpty]
    public List<string> SupportedTargetPlatforms { get; set; } = [];

    /// <summary>
    /// List of programs supported by this plugin.
    /// </summary>
    [JsonPropertyName("SupportedPrograms")]
    [DefaultAsEmpty]
    public List<string> SupportedPrograms { get; set; } = [];

    /// <summary>
    /// List of all modules associated with this plugin
    /// </summary>
    [JsonPropertyName("Modules")]
    [DefaultAsEmpty]
    public List<ModuleDescriptor> Modules { get; set; } = [];

    /// <summary>
    /// List of all localization targets associated with this plugin
    /// </summary>
    [JsonPropertyName("LocalizationTargets")]
    [DefaultAsEmpty]
    public List<LocalizationTargetDescriptor> LocalizationTargets { get; set; } = [];

    /// <summary>
    /// The Verse path to the root of this plugin's content directory
    /// </summary>
    [JsonPropertyName("VersePath")]
    public string? VersePath { get; set; }

    /// <summary>
    /// Origin/visibility of Verse code in this plugin's Content/Verse folder
    /// </summary>
    [JsonPropertyName("VerseScope")]
    [DefaultValue(VerseScope.PublicUser)]
    public VerseScope VerseScope { get; set; } = VerseScope.PublicUser;

    /// <summary>
    /// The version of the Verse language that this plugin targets.
    /// If no value is specified, the latest stable version is used.
    /// </summary>
    [JsonPropertyName("VerseVersion")]
    public uint? VerseVersion { get; set; }

    /// <summary>
    /// Whether this plugin should be enabled by default for all projects
    /// </summary>
    [JsonPropertyName("bEnabledByDefault")]
    public bool? EnabledByDefault { get; set; }

    /// <summary>
    /// Can this plugin contain content?
    /// </summary>
    [JsonPropertyName("bCanContainContent")]
    [DefaultValue(false)]
    public bool CanContainContent { get; set; } = false;

    /// <summary>
    /// Can this plugin contain Verse code (either in content directory or in any of its modules)?
    /// </summary>
    [JsonPropertyName("bCanContainVerse")]
    [DefaultValue(false)]
    public bool CanContainVerse { get; set; } = false;

    /// <summary>
    /// Marks the plugin as beta in the UI
    /// </summary>
    [JsonPropertyName("bIsBetaVersion")]
    [DefaultValue(false)]
    public bool IsBetaVersion { get; set; } = false;

    /// <summary>
    /// Marks the plugin as experimental in the UI
    /// </summary>
    [JsonPropertyName("bIsExperimentalVersion")]
    [DefaultValue(false)]
    public bool IsExperimentalVersion { get; set; } = false;

    /// <summary>
    /// Set for plugins which are installed
    /// </summary>
    [JsonPropertyName("bInstalled")]
    [DefaultValue(false)]
    public bool Installed { get; set; } = false;

    /// <summary>
    /// For plugins that are under a platform folder (eg. /IOS/), determines whether compiling the plugin requires the build platform and/or SDK to be available
    /// </summary>
    [JsonPropertyName("bRequiresBuildPlatform")]
    [DefaultValue(false)]
    public bool RequiresBuildPlatform { get; set; } = false;

    /// <summary>
    /// When true, prevents other plugins from depending on this plugin
    /// </summary>
    [JsonPropertyName("bIsSealed")]
    [DefaultValue(false)]
    public bool IsSealed { get; set; } = false;

    /// <summary>
    /// When true, this plugin should not contain any code or modules.
    /// </summary>
    [JsonPropertyName("bNoCode")]
    [DefaultValue(false)]
    public bool NoCode { get; set; } = false;

    /// <summary>
    /// When true, this plugin's modules will not be loaded automatically nor will it's content be mounted automatically. It will load/mount when explicitly requested and LoadingPhases will be ignored
    /// </summary>
    [JsonPropertyName("bExplicitlyLoaded")]
    [DefaultValue(false)]
    public bool ExplicitlyLoaded { get; set; } = false;

    /// <summary>
    /// When true, an empty SupportedTargetPlatforms is interpreted as 'no platforms' with the expectation that explicit platforms will be added in plugin platform extensions
    /// </summary>
    [JsonPropertyName("bHasExplicitPlatforms")]
    [DefaultValue(false)]
    public bool HasExplicitPlatforms { get; set; } = false;

    /// <summary>
    /// Set of pre-build steps to execute, keyed by host platform name.
    /// </summary>
    [JsonPropertyName("PreBuildSteps")]
    [DefaultAsEmpty]
    public CustomBuildSteps PreBuildSteps { get; set; } = new();

    /// <summary>
    /// Set of post-build steps to execute, keyed by host platform name.
    /// </summary>
    [JsonPropertyName("PostBuildSteps")]
    [DefaultAsEmpty]
    public CustomBuildSteps PostBuildSteps { get; set; } = new();

    /// <summary>
    /// Additional plugins that this plugin depends on
    /// </summary>
    [JsonPropertyName("Plugins")]
    [DefaultAsEmpty]
    public List<PluginReferenceDescriptor> Plugins { get; set; } = [];

    /// <summary>
    /// Plugins that this plugin should never depend on
    /// </summary>
    [JsonPropertyName("DisallowedPlugins")]
    [DefaultAsEmpty]
    public List<string> DisallowedPlugins { get; set; } = [];
}