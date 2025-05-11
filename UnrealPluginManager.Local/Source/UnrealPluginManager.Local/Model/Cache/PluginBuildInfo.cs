using System.Text.Json.Serialization;
using Semver;
using UnrealPluginManager.Core.Converters;

namespace UnrealPluginManager.Local.Model.Cache;

/// <summary>
/// Represents detailed build information for a specific plugin, including metadata, versioning,
/// build environment, and platform compatibility. This information is used for tracking and managing
/// built plugins within caching systems or plugin managers.
/// </summary>
public record PluginBuildInfo {
  /// <summary>
  /// Unique identifier for the plugin build entity.
  /// Used to distinguish individual builds within the plugin management system.
  /// </summary>
  public Guid Id { get; init; }

  /// <summary>
  /// Represents the name of the plugin associated with the build info.
  /// Used for identifying and referring to the plugin within various operations
  /// such as installation, mapping, and cache management.
  /// </summary>
  public required string PluginName { get; init; }

  /// <summary>
  /// Represents the version of the plugin being managed.
  /// Utilized for version control, dependency resolution, and ensuring compatibility with specific engine versions.
  /// </summary>
  public required SemVersion PluginVersion { get; init; }

  /// <summary>
  /// Specifies the version of the engine associated with the plugin build.
  /// Used to determine compatibility between the plugin and the engine version.
  /// </summary>
  public required string EngineVersion { get; init; }

  /// <summary>
  /// Specifies the list of supported platforms for the plugin build.
  /// Used to determine the compatibility of the plugin with various platforms during deployment or usage.
  /// </summary>
  public List<string> Platforms { get; init; } = [];

  /// <summary>
  /// Represents the directory path where the plugin is built or cached locally.
  /// Used for identifying and accessing the location of the plugin build files.
  /// </summary>
  public required string DirectoryName { get; init; }

  /// <summary>
  /// Timestamp indicating when the plugin build was created.
  /// Provides a chronological reference for tracking and organizing builds.
  /// </summary>
  public required DateTimeOffset BuiltOn { get; init; }

  /// <summary>
  /// Dictionary mapping build dependencies to their corresponding semantic versions.
  /// Provides detailed versioning information about the tools and environments used to build the plugin.
  /// Useful for ensuring compatibility and tracking build dependencies.
  /// </summary>
  [JsonConverter(typeof(StringKeyDictionaryConverter<SemVersion>))]
  public Dictionary<string, SemVersion> BuiltWith { get; init; } = [];
}