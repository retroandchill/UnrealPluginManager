using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Dunet;
using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Model.Resolution;

/// <summary>
/// Represents the result of a resolution process that determines the required
/// or conflicting plugins within a dependency chain.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ResolvedDependencies), typeDiscriminator: nameof(ResolvedDependencies))]
[JsonDerivedType(typeof(ConflictDetected), typeDiscriminator: nameof(ConflictDetected))]
[Union]
public partial record ResolutionResult {
  /// <summary>
  /// Represents a resolution outcome where all required dependencies have been successfully resolved.
  /// </summary>
  /// <remarks>
  /// This class encapsulates the selected plugins as part of a successful dependency resolution process.
  /// </remarks>
  public partial record ResolvedDependencies(List<PluginSummary> SelectedPlugins);

  /// <summary>
  /// Represents a conflict detected during the resolution process
  /// when plugin dependencies have incompatible requirements.
  /// </summary>
  /// <remarks>
  /// This record captures the details of the conflicts, including the plugins and their
  /// respective conflicting requirements. It enables analyzing and resolving
  /// dependency-related issues within the resolution process.
  /// </remarks>
  public partial record ConflictDetected(List<Conflict> Conflicts);
  
}