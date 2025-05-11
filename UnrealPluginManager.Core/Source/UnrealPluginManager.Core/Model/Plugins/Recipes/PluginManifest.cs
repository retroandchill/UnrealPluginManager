using System.Text.Json.Serialization;
using Semver;
using UnrealPluginManager.Core.Converters;

namespace UnrealPluginManager.Core.Model.Plugins.Recipes;

/// <summary>
/// Represents a manifest for a plugin, containing metadata and configuration needed to identify
/// and manage the plugin within the Unreal Plugin Manager.
/// </summary>
public record PluginManifest {

  /// <summary>
  /// Gets the name of the plugin as defined in its manifest. This property is
  /// required and serves as a unique identifier for the plugin within the Unreal Plugin Manager.
  /// </summary>
  public required string Name { get; init; }

  /// <summary>
  /// Gets the semantic version of the plugin as defined in its manifest.
  /// This property specifies the version of the plugin for identification, compatibility, and dependency resolution purposes.
  /// </summary>
  [JsonConverter(typeof(SemVersionJsonConverter))]
  public required SemVersion Version { get; init; }

  /// <summary>
  /// Gets or sets the author of the plugin. This property is optional and provides
  /// information about the individual or organization responsible for creating
  /// or maintaining the plugin.
  /// </summary>
  public string? Author { get; init; }

  /// <summary>
  /// Provides a detailed description of the plugin. This property is optional and may include
  /// any relevant information about the plugin, such as its purpose, features, or any additional
  /// context needed for users or developers.
  /// </summary>
  public string? Description { get; init; }

  /// <summary>
  /// Gets or sets the license associated with the plugin. This property provides information
  /// about the licensing terms under which the plugin is distributed.
  /// </summary>
  public string? License { get; init; }

  /// <summary>
  /// Gets the homepage URL of the plugin. This property provides a link to the plugin's
  /// main page or website containing additional information, updates, or documentation.
  /// </summary>
  public Uri? Homepage { get; init; }

  /// <summary>
  /// Gets the source location of the plugin. This property typically includes the
  /// URL and commit SHA that uniquely identifies the plugin's source in a repository.
  /// </summary>
  public required SourceLocation Source { get; init; }

  /// <summary>
  /// Gets the list of dependencies required by the plugin. Each dependency is described
  /// by its name, version range, and optional repository source from which it can be retrieved.
  /// This property ensures that all necessary plugins or components are available and aligned
  /// with the plugin's requirements, allowing proper functionality within the Unreal Plugin Manager.
  /// </summary>
  public required List<PluginDependencyManifest> Dependencies { get; init; }

}