using System.Runtime.Versioning;
using UnrealPluginManager.Core.Abstractions;

namespace UnrealPluginManager.Cli.Tests.Mocks;

[SupportedOSPlatform("windows")]
public static class MockRegistryUtils {
    
    public static MockRegistryKey OpenOrAddSubKey(this IRegistryKey key, string name) {
        var castedKey = (MockRegistryKey) key;
        if (castedKey is null) {
            throw new ArgumentException("Key is not a MockRegistryKey");
        }
        IRegistryKey? subkeyValue = null;
        foreach (var subkey in name.Split('\\')) {
            if (!castedKey.SubKeys.TryGetValue(subkey, out subkeyValue)) {
                subkeyValue = new MockRegistryKey();
                castedKey.SubKeys.Add(subkey, subkeyValue);
            }
            
            castedKey = (MockRegistryKey) subkeyValue;
        }
        
        return (MockRegistryKey) subkeyValue!;
    }
    
    public static void SetValue(this IRegistryKey key, string name, object value) {
        var castedKey = (MockRegistryKey) key;
        if (castedKey is null) {
            throw new ArgumentException("Key is not a MockRegistryKey");
        }
        castedKey.Values.Add(name, value);
    }
    
}