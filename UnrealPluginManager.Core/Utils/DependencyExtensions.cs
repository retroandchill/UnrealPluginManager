using Semver;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Utils;

/// <summary>
/// Provides extension methods for managing and processing plugin dependency manifests within the UnrealPluginManager system.
/// </summary>
/// <remarks>
/// This static class contains utility methods for aggregating, modifying, and enriching <see cref="DependencyManifest"/> objects,
/// such as collapsing multiple manifests into a single one, marking dependencies as installed, or updating remote index information.
/// </remarks>
public static class DependencyExtensions {
    
    /// <summary>
    /// Combines multiple <see cref="DependencyManifest"/> instances into a single manifest,
    /// merging their dependencies and resolving any overlaps.
    /// </summary>
    /// <param name="manifests">An asynchronous enumerable of <see cref="DependencyManifest"/> instances to merge.</param>
    /// <returns>A single <see cref="DependencyManifest"/> containing the merged dependencies and resolved items.</returns>
    public static async Task<DependencyManifest> Collapse(this IAsyncEnumerable<DependencyManifest> manifests) {
        var result = new DependencyManifest();
        await foreach (var manifest in manifests) {
            foreach (var (name, dependencies) in manifest.FoundDependencies) {
                var currentList = result.FoundDependencies.ComputeIfAbsent(name, _ => []);
                currentList.AddDistinctRange(dependencies, x => x.Version);
            }
            
            result.UnresolvedDependencies.AddRange(manifest.UnresolvedDependencies);
        }
        
        result.UnresolvedDependencies.RemoveWhere(x => result.FoundDependencies.ContainsKey(x));
        return result;
    }

    /// <summary>
    /// Updates the provided <see cref="DependencyManifest"/> instance by marking dependencies
    /// as installed if their version matches the version found in the installed plugins dictionary.
    /// </summary>
    /// <param name="manifest">The <see cref="DependencyManifest"/> containing the dependencies to check and update.</param>
    /// <param name="installedPlugins">
    /// A dictionary where the key is the name of an installed plugin, and the value is its associated <see cref="SemVersion"/>.
    /// </param>
    /// <returns>
    /// The updated <see cref="DependencyManifest"/> with dependencies marked as installed when a match is found.
    /// </returns>
    public static DependencyManifest SetInstalledPlugins(this DependencyManifest manifest,
                                                         IReadOnlyDictionary<string, SemVersion> installedPlugins) {
        foreach (var dependency in manifest.FoundDependencies.Values.SelectMany(x => x)) {
            if (installedPlugins.GetValueOrDefault(dependency.Name) == dependency.Version) {
                dependency.Installed = true;
            }
        }

        return manifest;
    }

    /// <summary>
    /// Updates the <see cref="DependencyManifest"/> by setting the specified remote index
    /// for all dependencies within the manifest.
    /// </summary>
    /// <param name="manifest">The <see cref="DependencyManifest"/> to update.</param>
    /// <param name="remoteIndex">The remote index value to set for all dependencies.</param>
    /// <returns>The updated <see cref="DependencyManifest"/> with the specified remote index applied to its dependencies.</returns>
    public static DependencyManifest SetRemoteIndex(this DependencyManifest manifest, int remoteIndex) {
        foreach (var dependency in manifest.FoundDependencies.Values.SelectMany(x => x)) {
            dependency.RemoteIndex = remoteIndex;
        }
        
        return manifest;
    }

}