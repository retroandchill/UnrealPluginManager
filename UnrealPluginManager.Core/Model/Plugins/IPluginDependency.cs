using Semver;

namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Defines the contract for plugin dependency metadata in the Unreal Plugin Manager system.
/// </summary>
/// <remarks>
/// An implementation of IPluginDependency provides details about a plugin
/// that another plugin depends on. The information includes the dependent
/// plugin's name, the type of the plugin, and a version range specifying
/// compatible versions of the dependent plugin.
/// </remarks>
public interface IPluginDependency {

    /// <summary>
    /// Gets the name of the plugin.
    /// </summary>
    /// <remarks>
    /// The name represents the unique identifier for the plugin and must conform to
    /// specified naming conventions, such as starting with an uppercase letter and
    /// not including whitespace. This property is required and has a length
    /// limitation between 1 and 255 characters.
    /// </remarks>
    string PluginName { get; }

    /// <summary>
    /// Gets the type of the plugin.
    /// </summary>
    /// <remarks>
    /// The type determines the classification of the plugin, which may include
    /// categories such as Engine, Provided, or External. This property is utilized
    /// to differentiate plugins based on their origin or functionality and
    /// typically influences dependency evaluation or management within the system.
    /// </remarks>
    PluginType Type { get; }

    /// <summary>
    /// Gets the version range of the dependent plugin.
    /// </summary>
    /// <remarks>
    /// The version range specifies the compatible versions of the plugin that
    /// this dependency applies to. It supports semantic versioning constraints
    /// and allows for defining a range or specific version that meets the compatibility requirements.
    /// </remarks>
    SemVersionRange PluginVersion { get;  }
    
}