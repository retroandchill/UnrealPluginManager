using System.Runtime.Versioning;
using Microsoft.Win32;

namespace UnrealPluginManager.Cli.Abstractions;

/// <summary>
/// Represents a wrapper for a Windows registry key, providing a concrete
/// implementation of the <see cref="IRegistryKey"/> interface.
/// </summary>
/// <remarks>
/// This class is designed for interacting with Windows registry keys, allowing
/// access to subkeys, their names, values, and data. It is intended to be used
/// on Windows platforms only, as indicated by the platform support attribute.
/// </remarks>
[SupportedOSPlatform("windows")]
public record WindowsRegistryKey(RegistryKey RegistryKey) : IRegistryKey {
    /// <inheritdoc />
    public IRegistryKey? OpenSubKey(string name) {
        var key = RegistryKey.OpenSubKey(name);
        return key is not null ? new WindowsRegistryKey(key) : null;
    }

    /// <inheritdoc />
    public string[] GetSubKeyNames() {
        return RegistryKey.GetSubKeyNames();
    }

    /// <inheritdoc />
    public string[] GetValueNames() {
        return RegistryKey.GetValueNames();
    }

    /// <inheritdoc />
    public T? GetValue<T>(string name) {
        return (T?) RegistryKey.GetValue(name);
    }
}