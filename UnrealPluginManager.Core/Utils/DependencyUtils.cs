using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Utils;

public static class DependencyUtils {

    public static DependencyManifest Merge(this DependencyManifest source, DependencyManifest other) {
        var result = new DependencyManifest {
            FoundDependencies = source.FoundDependencies.ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
        };
        
        foreach (var (name, versions) in other.FoundDependencies) {
            if (source.FoundDependencies.TryGetValue(name, out var existingVersions)) {
                result.FoundDependencies[name] = existingVersions.UnionBy(versions, v => (v.Name, v.Version)).ToList();
            } else {
                result.FoundDependencies[name] = versions;
            }
        }
        
        result.UnresolvedDependencies = source.UnresolvedDependencies.Union(other.UnresolvedDependencies)
            .Where(result.FoundDependencies.ContainsKey)
            .ToHashSet();
        return result;
    }
    
}