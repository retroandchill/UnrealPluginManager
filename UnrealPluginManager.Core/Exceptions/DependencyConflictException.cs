using LanguageExt;
using UnrealPluginManager.Core.Model.Resolution;

namespace UnrealPluginManager.Core.Exceptions;

/// <summary>
/// Represents an exception that is thrown when there are conflicts while resolving dependencies
/// within the Unreal Plugin Manager.
/// </summary>
/// <remarks>
/// DependencyConflictException is used to encapsulate and convey information about dependency
/// resolution conflicts that occur when attempting to resolve plugins with incompatible requirements.
/// It includes a collection of conflicts that provides detailed information about each conflicting case.
/// </remarks>
public class DependencyConflictException(IReadOnlyList<Conflict> conflicts, Exception? innerException = null)
    : UnrealPluginManagerException("There were conflicts trying to resolve the dependencies!", innerException) {

  /// <summary>
  /// Gets the collection of dependency conflicts that caused the exception to be thrown.
  /// </summary>
  /// <remarks>
  /// Each conflict provides information about a specific plugin and the incompatible requirements
  /// that led to the resolution issue. This property is useful for diagnosing dependency conflicts
  /// and understanding the root cause of the failure during dependency resolution.
  /// </remarks>
  public IReadOnlyList<Conflict> Conflicts { get; } = conflicts;

}