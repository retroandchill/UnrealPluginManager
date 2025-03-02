using Semver;

namespace UnrealPluginManager.Core.Solver;

/// <summary>
/// Represents a selected version with a name and its corresponding semantic version.
/// </summary>
/// <remarks>
/// This struct is used to encapsulate the name of a component (e.g., plugin, dependency)
/// and its resolved version as a semantic version.
/// </remarks>
public readonly record struct SelectedVersion(string Name, SemVersion Version) : IComparable<SelectedVersion> {

    /// <inheritdoc />
    public int CompareTo(SelectedVersion other) {
        var nameComparison = string.Compare(Name, other.Name, StringComparison.InvariantCultureIgnoreCase);
        return nameComparison != 0 ? nameComparison : Version.ComparePrecedenceTo(other.Version);

    }
}