using System.Text.Json.Serialization;
using UnrealPluginManager.Core.Meta;
using UnrealPluginManager.Core.Model.EngineFile;
using UnrealPluginManager.Core.Model.Modules;
using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Model.Project;

using CustomBuildSteps = Dictionary<string, List<string>>;

/// <summary>
/// Represents the descriptor for an Unreal Engine project, defining its metadata,
/// modules, plugins, supported platforms, and custom build steps.
/// </summary>
public class ProjectDescriptor : IDependencyHolder {
    /// <summary>
    /// Descriptor version number.
    /// </summary>
    [JsonPropertyName("FileVersion")]
    public int FileVersion { get; set; }

    /// <summary>
    /// The engine to open this project with.
    /// </summary>
    [JsonPropertyName("EngineAssociation")]
    public string? EngineAssociation { get; set; }

    /// <summary>
    /// Category to show under the project browser
    /// </summary>
    [JsonPropertyName("Category")]
    public string? Category { get; set; }

    /// <summary>
    /// Description to show in the project browser
    /// </summary>
    [JsonPropertyName("Description")]
    public string? Description { get; set; }

    /// <summary>
    /// List of all modules associated with this project
    /// </summary>
    [JsonPropertyName("Modules")]
    [DefaultAsEmpty]
    public List<ModuleDescriptor> Modules { get; set; } = [];

    /// <summary>
    /// List of plugins for this project (may be enabled/disabled)
    /// </summary>
    [JsonPropertyName("Plugins")]
    [DefaultAsEmpty]
    public List<PluginReferenceDescriptor> Plugins { get; set; } = [];

    /// <summary>
    /// Array of additional root directories
    /// </summary>
    [JsonPropertyName("AdditionalRootDirectories")]
    [DefaultAsEmpty]
    public List<string> AdditionalRootDirectories { get; set; } = [];

    /// <summary>
    /// List of additional plugin directories to scan for available plugins
    /// </summary>
    [JsonPropertyName("AdditionalPluginDirectories")]
    [DefaultAsEmpty]
    public List<string> AdditionalPluginDirectories { get; set; } = [];

    /// <summary>
    /// Array of platforms that this project is targeting
    /// </summary>
    [JsonPropertyName("SupportedTargetPlatforms")]
    [DefaultAsEmpty]
    public List<string> SupportedTargetPlatforms { get; set; } = [];

    /// <summary>
    /// A hash that is used to determine if the project was forked from a sample
    /// </summary>
    [JsonPropertyName("EpicSampleNameHash")]
    public uint EpicSampleNameHash { get; set; }

    /// <summary>
    /// Steps to execute before creating rules assemblies in this project
    /// </summary>
    [JsonPropertyName("InitSteps")]
    [DefaultAsEmpty]
    public CustomBuildSteps InitSteps { get; set; } = new();

    /// <summary>
    /// Steps to execute before building targets in this project
    /// </summary>
    [JsonPropertyName("PreBuildSteps")]
    [DefaultAsEmpty]
    public CustomBuildSteps PreBuildSteps { get; set; } = new();

    /// <summary>
    /// Steps to execute before building targets in this project
    /// </summary>
    [JsonPropertyName("PostBuildSteps")]
    [DefaultAsEmpty]
    public CustomBuildSteps PostBuildSteps { get; set; } = new();

    /// <summary>
    /// Indicates if this project is an Enterprise project
    /// </summary>
    [JsonPropertyName("IsEnterpriseProject")]
    public bool IsEnterpriseProject { get; set; } = false;

    /// <summary>
    /// Indicates that enabled by default engine plugins should not be enabled unless explicitly enabled by the project or target files.
    /// </summary>
    [JsonPropertyName("DisableEnginePluginsByDefault")]
    public bool DisableEnginePluginsByDefault { get; set; } = false;
}