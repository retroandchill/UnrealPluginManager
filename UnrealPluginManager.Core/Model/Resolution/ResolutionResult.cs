using System.ComponentModel;
using System.Text.Json.Serialization;
using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Model.Resolution;

/// <summary>
/// Represents the result of a plugin resolution operation within the Unreal Plugin Manager.
/// </summary>
/// <remarks>
/// This interface serves as a base type for the outcomes of resolving plugin dependencies. It can represent either:
/// - Successfully resolved plugin dependencies with a list of selected plugins.
/// - Detected conflicts during the resolution process with a list of conflicting requirements.
/// Derived types include:
/// - <see cref="ResolvedDependencies"/>: Represents a successful resolution with a list of selected plugins.
/// - <see cref="ConflictDetected"/>: Represents resolution failure due to conflicts with a description of the conflicts.
/// </remarks>
[JsonDerivedType(typeof(ResolvedDependencies), typeDiscriminator: "Resolved")]
[JsonDerivedType(typeof(ConflictDetected), typeDiscriminator: "ConflictsDetected")]
public abstract class ResolutionResult {
  /// <summary>
  /// Defines an implicit conversion operator for resolving plugin summaries into a
  /// <see cref="ResolutionResult"/>.
  /// </summary>
  /// <remarks>
  /// This operator allows a list of <see cref="PluginSummary"/> objects to be implicitly cast
  /// to a <see cref="ResolutionResult"/>. When this conversion is performed, an instance of
  /// <see cref="ResolvedDependencies"/> is created, which contains the given list of plugins as its resolved dependencies.
  /// </remarks>
  /// <param name="summaries">The list of plugins to be converted into a resolution result.</param>
  /// <returns>
  /// A <see cref="ResolvedDependencies"/> instance containing the provided summaries as the resolved plugins.
  /// </returns>
  public static implicit operator ResolutionResult(List<PluginSummary> summaries) {
    return new ResolvedDependencies { SelectedPlugins = summaries };
  }

  /// <summary>
  /// Defines an implicit conversion operator for transforming a list of conflicts into a
  /// <see cref="ResolutionResult"/>.
  /// </summary>
  /// <remarks>
  /// This operator allows a collection of <see cref="Conflict"/> objects to be implicitly converted
  /// to a <see cref="ResolutionResult"/>. Specifically, this results in the creation of a
  /// <see cref="ConflictDetected"/> instance, which encapsulates the provided conflicts.
  /// </remarks>
  /// <param name="conflicts">The collection of conflicts that need to be encapsulated in the resolution result.</param>
  /// <returns>
  /// A <see cref="ConflictDetected"/> instance that holds the given list of conflicts.
  /// </returns>
  public static implicit operator ResolutionResult(List<Conflict> conflicts) {
    return new ConflictDetected { Conflicts = conflicts };
  }
  
}

/// <summary>
/// Represents a successful resolution of plugin dependencies within the Unreal Plugin Manager.
/// </summary>
/// <remarks>
/// This class is a concrete implementation of <see cref="ResolutionResult"/> and signifies that all plugin dependencies
/// have been resolved without conflicts. It provides a list of selected plugins that meet the resolution criteria.
/// </remarks>
public class ResolvedDependencies : ResolutionResult {
  /// <summary>
  /// Gets or sets the list of plugins that have been successfully resolved during dependency resolution.
  /// </summary>
  /// <remarks>
  /// This property contains instances of <see cref="PluginSummary"/> that represent the plugins
  /// selected as part of a successful resolution. Each plugin provides details such as its ID, name, version,
  /// and optional friendly name. This ensures the resolved dependencies are accurately captured
  /// for further use in the Unreal Plugin Manager workflow.
  /// </remarks>
  public required List<PluginSummary> SelectedPlugins { get; set; }
  
}

/// <summary>
/// Represents resolution failure due to detected conflicts among plugin dependencies within the Unreal Plugin Manager.
/// </summary>
/// <remarks>
/// This class is a concrete implementation of <see cref="ResolutionResult"/> and indicates that plugin dependencies
/// could not be resolved because of conflicts between required versions. It provides a detailed list of conflicts
/// that must be addressed to successfully resolve the dependencies.
/// </remarks>
public class ConflictDetected : ResolutionResult {
  /// <summary>
  /// Gets or sets the list of conflicts detected during the dependency resolution process in the Unreal Plugin Manager.
  /// </summary>
  /// <remarks>
  /// This property contains a collection of <see cref="Conflict"/> objects, where each conflict details the plugin name
  /// alongside the version requirements imposed by different sources. These conflicts represent incompatible version
  /// dependencies that need to be resolved to proceed with successful dependency resolution.
  /// </remarks>
  public required List<Conflict> Conflicts { get; set; }
  
}