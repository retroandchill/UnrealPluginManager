using System.Text.Json;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Plugins;

namespace UnrealPluginManager.Core.Utils;

public static class PluginUtils {

    private static readonly JsonSerializerOptions JsonOptions = new() {
        AllowTrailingCommas = true
    };
    
    internal static T ResolvePluginName<T>(this Dictionary<string, T> foundPlugins, string pluginName) {
        try {
            return foundPlugins[pluginName.ToLower()];
        } catch (KeyNotFoundException e) {
            throw new DependencyResolutionException($"Unable to resolve dependency: {pluginName}", e);
        }
    }
    
    internal static (string, PluginDescriptor) ReadPluginDescriptorFromFile(string filePath) {
        using var fileStream = new StreamReader(filePath);
        var json = fileStream.ReadToEnd();
        var descriptor = JsonSerializer.Deserialize<PluginDescriptor>(json, JsonOptions);
        if (descriptor is null) {
            throw new JsonException($"Failed to read plugin descriptor for: {filePath}");
        }
        
        return (Path.GetFileNameWithoutExtension(filePath), descriptor);
    }
}