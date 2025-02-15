using System.Runtime.Versioning;
using UnrealPluginManager.Core.Abstractions;

namespace UnrealPluginManager.Cli.Tests.Mocks;

[SupportedOSPlatform("windows")]
public class MockRegistryKey : IRegistryKey {
    
    public Dictionary<string, IRegistryKey> SubKeys { get; set; } = new();
    public Dictionary<string, object> Values { get; set; } = new();
    
    public IRegistryKey? OpenSubKey(string name) {
        var splitString = name.Split('\\', 2);
        if (splitString.Length < 2) {
            return  SubKeys
                .Where(x => x.Key.Equals(splitString[0], StringComparison.OrdinalIgnoreCase))
                .Select(x => x.Value)
                .FirstOrDefault();;
        }
        var key = splitString[0];
        var remainder = splitString[1];
        var match = SubKeys
            .Where(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Value)
            .FirstOrDefault();
        return match?.OpenSubKey(remainder);
    }

    public string[] GetSubKeyNames() {
        return SubKeys.Keys.ToArray();
    }

    public string[] GetValueNames() {
        return Values.Keys.ToArray();
    }

    public T? GetValue<T>(string name) {
        return (T?) Values.GetValueOrDefault(name);
    }
}