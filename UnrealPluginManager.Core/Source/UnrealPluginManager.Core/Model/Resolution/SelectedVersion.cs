using Semver;

namespace UnrealPluginManager.Core.Model.Resolution;

/// <summary>
/// Represents a selected version with a name and its corresponding semantic version.
/// </summary>
/// <remarks>
/// This struct is used to encapsulate the name of a component (e.g., plugin, dependency)
/// and its resolved version as a semantic version.
/// </remarks>
public readonly record struct SelectedVersion(string Name, SemVersion Version) : IComparable<SelectedVersion> {
  /// <summary>
  /// Indicates whether the selected version is currently installed on the system.
  /// </summary>
  /// <remarks>
  /// This property is a boolean flag used to differentiate between installed and non-installed versions.
  /// It simplifies the comparison and resolution process when evaluating version dependencies or selecting
  /// the appropriate version for a plugin or component.
  /// </remarks>
  public bool Installed { get; init; } = false;

  /// <summary>
  /// Represents the index of the selected version in a remote data source, if applicable.
  /// </summary>
  /// <remarks>
  /// This property is used to track the position or identifier of a version in a remote repository or
  /// data source. It is nullable, allowing differentiation between versions that originate from a
  /// remote source and those that do not. When comparing versions, the `RemoteIndex` can influence the
  /// sorting or selection process by providing additional context about the version's provenance.
  /// </remarks>
  public int? RemoteIndex { get; init; } = null;

  /// <inheritdoc />
  public int CompareTo(SelectedVersion other) {
    var nameComparison = string.Compare(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
    if (nameComparison != 0) {
      return nameComparison;
    }

    var compareInstalled = Installed.CompareTo(other.Installed);
    if (compareInstalled != 0) {
      return compareInstalled;
    }

    var versionComparison = Version.ComparePrecedenceTo(other.Version);
    if (versionComparison != 0) {
      return versionComparison;
    }

    var xIndex = RemoteIndex.GetValueOrDefault(-1);
    var yIndex = RemoteIndex.GetValueOrDefault(-1);
    return xIndex.CompareTo(yIndex);
  }
}