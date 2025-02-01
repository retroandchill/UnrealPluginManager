using UnrealPluginManager.Core.Exceptions;

namespace UnrealPluginManager.Core.Utils;

public static class PluginUtils {
    internal static T ResolvePluginName<T>(this Dictionary<string, T> foundPlugins, string pluginName) {
        try {
            return foundPlugins[pluginName];
        } catch (KeyNotFoundException e) {
            throw new DependencyResolutionException($"Unable to resolve dependency: {pluginName}", e);
        }
    }
}