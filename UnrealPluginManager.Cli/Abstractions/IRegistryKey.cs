using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using Microsoft.Win32;

namespace UnrealPluginManager.Cli.Abstractions;

[SupportedOSPlatform("windows")]
public interface IRegistryKey : IDisposable {
    string Name { get; }
    
    int SubKeyCount { get; }
    
    RegistryView View { get; }
    
    ISafeRegistryHandle Handle { get; }
    
    int ValueCount { get; }
    
    void Flush();

    void Close();

    IRegistryKey CreateSubKey(string subkey);

    IRegistryKey CreateSubKey(string subkey, bool writable);

    IRegistryKey CreateSubKey(string subkey, bool writable, RegistryOptions options);

    IRegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck);

    IRegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck,
        RegistryOptions registryOptions, IRegistrySecurity? registrySecurity);

    IRegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck,
        RegistryOptions registryOptions);

    void DeleteSubKey(string subkey);

    void DeleteSubKey(string subkey, bool throwOnMissingSubKey);

    void DeleteSubKeyTree(string subkey);

    void DeleteSubKeyTree(string subkey, bool throwOnMissingSubKey);

    void DeleteValue(string name);

    void DeleteValue(string name, bool throwOnMissingValue);

    IRegistryKey? OpenSubKey(string name);

    IRegistryKey? OpenSubKey(string name, bool writable);

    IRegistryKey? OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck);

    IRegistryKey? OpenSubKey(string name, RegistryRights rights);

    IRegistryKey? OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck, RegistryRights rights);

    IRegistrySecurity GetAccessControl();

    IRegistrySecurity GetAccessControl(AccessControlSections includeSections);

    void SetAccessControl(IRegistrySecurity registrySecurity);

    string[] GetSubKeyNames();

    string[] GetValueNames();

    object? GetValue(string? name);

    [return: NotNullIfNotNull(nameof(defaultValue))]
    object? GetValue(string? name, object? defaultValue);

    [return: NotNullIfNotNull(nameof(defaultValue))]
    object? GetValue(string? name, object? defaultValue, RegistryValueOptions options);

    RegistryValueKind GetValueKind(string? name);

    void SetValue(string? name, object value);

    void SetValue(string? name, object value, RegistryValueKind valueKind);
}