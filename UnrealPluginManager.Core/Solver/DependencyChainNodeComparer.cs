using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Solver;

/// <summary>
/// Provides logic for comparing two instances of <see cref="IDependencyChainNode"/>.
/// This comparer implements custom comparison based on properties such as installation status,
/// version precedence, and remote index value in a prioritized order.
/// </summary>
/// <remarks>
/// The comparison is performed in the following order:
/// 1. Installed status: Nodes installed locally are prioritized over others.
/// 2. Version precedence: Compares versions using the <c>ComparePrecedenceTo</c> method.
/// 3. Remote index value: Compares nodes based on the optional remote index when other criteria are equal.
/// The implementation handles null values for the nodes and optional remote index by defining a consistent ordering.
/// </remarks>
public class DependencyChainNodeComparer : IComparer<IDependencyChainNode> {

    /// <inheritdoc />
    public int Compare(IDependencyChainNode? x, IDependencyChainNode? y) {
        if (ReferenceEquals(x, y)) {
            // Should handle the null case
            return 0;
        }

        if (x is null) {
            return -1;
        }

        if (y is null) {
            return 1;
        }

        var compareInstalled = x.Installed.CompareTo(y.Installed);
        if (compareInstalled != 0) {
            return compareInstalled;
        }

        var versionComparison = x.Version.ComparePrecedenceTo(y.Version);
        if (versionComparison != 0) {
            return versionComparison;
        }

        var xIndex = x.RemoteIndex.GetValueOrDefault(-1);
        var yIndex = y.RemoteIndex.GetValueOrDefault(-1);
        return xIndex.CompareTo(yIndex);
    }
}