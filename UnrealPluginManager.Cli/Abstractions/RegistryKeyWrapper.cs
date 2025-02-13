using System.Diagnostics.CodeAnalysis;
using System.Runtime.Versioning;
using System.Security.AccessControl;
using Microsoft.Win32;

namespace UnrealPluginManager.Cli.Abstractions;

[SupportedOSPlatform("windows")]
internal record RegistryKeyWrapper(RegistryKey RealKey) : IRegistryKey {

    public string Name => RealKey.Name;
    public int SubKeyCount => RealKey.SubKeyCount;
    public RegistryView View => RealKey.View;
    public ISafeRegistryHandle Handle => new SafeRegistryHandleWrapper(RealKey.Handle);
    public int ValueCount => RealKey.ValueCount;

    private static RegistryKeyWrapper? OfNullable(RegistryKey? key) {
        return key is null ? null : new RegistryKeyWrapper(key);
    }
    
    public void Dispose() {
        RealKey.Dispose();
        GC.SuppressFinalize(this);
    }

    public void Flush() => RealKey.Flush();

    public void Close() => RealKey.Close();

    public IRegistryKey CreateSubKey(string subkey) {
        return new RegistryKeyWrapper(RealKey.CreateSubKey(subkey));
    }

    public IRegistryKey CreateSubKey(string subkey, bool writable) {
        return new RegistryKeyWrapper(RealKey.CreateSubKey(subkey, writable));
    }

    public IRegistryKey CreateSubKey(string subkey, bool writable, RegistryOptions options) {
        return new RegistryKeyWrapper(RealKey.CreateSubKey(subkey, writable, options));
    }

    public IRegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck) {
        return new RegistryKeyWrapper(RealKey.CreateSubKey(subkey, permissionCheck));
    }

    public IRegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions registryOptions,
        IRegistrySecurity? registrySecurity) {
        return new RegistryKeyWrapper(RealKey.CreateSubKey(subkey, permissionCheck, registryOptions,
            registrySecurity?.RegistrySecurity));
    }

    public IRegistryKey CreateSubKey(string subkey, RegistryKeyPermissionCheck permissionCheck, RegistryOptions registryOptions) {
        return new RegistryKeyWrapper(RealKey.CreateSubKey(subkey, permissionCheck, registryOptions));
    }

    public void DeleteSubKey(string subkey) {
        RealKey.DeleteSubKey(subkey);
    }

    public void DeleteSubKey(string subkey, bool throwOnMissingSubKey) {
        RealKey.DeleteSubKey(subkey, throwOnMissingSubKey);
    }

    public void DeleteSubKeyTree(string subkey) {
        RealKey.DeleteSubKeyTree(subkey);
    }

    public void DeleteSubKeyTree(string subkey, bool throwOnMissingSubKey) {
        RealKey.DeleteSubKeyTree(subkey, throwOnMissingSubKey);
    }

    public void DeleteValue(string name) {
        RealKey.DeleteValue(name);
    }

    public void DeleteValue(string name, bool throwOnMissingValue) {
        RealKey.DeleteValue(name, throwOnMissingValue);
    }

    public IRegistryKey? OpenSubKey(string name) {
        return OfNullable(RealKey.OpenSubKey(name));
    }

    public IRegistryKey? OpenSubKey(string name, bool writable) {
        return OfNullable(RealKey.OpenSubKey(name, writable));
    }

    public IRegistryKey? OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck) {
        return OfNullable(RealKey.OpenSubKey(name, permissionCheck));
    }

    public IRegistryKey? OpenSubKey(string name, RegistryRights rights) {
        return OfNullable(RealKey.OpenSubKey(name, rights));
    }

    public IRegistryKey? OpenSubKey(string name, RegistryKeyPermissionCheck permissionCheck, RegistryRights rights) {
        return OfNullable(RealKey.OpenSubKey(name, permissionCheck, rights));
    }

    public IRegistrySecurity GetAccessControl() {
        return new RegistrySecurityWrapper(RealKey.GetAccessControl());
    }

    public IRegistrySecurity GetAccessControl(AccessControlSections includeSections) {
        return new RegistrySecurityWrapper(RealKey.GetAccessControl(includeSections));
    }

    public void SetAccessControl(IRegistrySecurity registrySecurity) {
        RealKey.SetAccessControl(registrySecurity.RegistrySecurity);
    }

    public string[] GetSubKeyNames() {
        return RealKey.GetSubKeyNames();
    }

    public string[] GetValueNames() {
        return RealKey.GetValueNames();
    }

    public object? GetValue(string? name) {
        return RealKey.GetValue(name);
    }

    [return: NotNullIfNotNull("defaultValue")]
    public object? GetValue(string? name, object? defaultValue) {
        return RealKey.GetValue(name, defaultValue);
    }

    [return: NotNullIfNotNull("defaultValue")]
    public object? GetValue(string? name, object? defaultValue, RegistryValueOptions options) {
        return RealKey.GetValue(name, defaultValue, options);
    }

    public RegistryValueKind GetValueKind(string? name) {
        return RealKey.GetValueKind(name);
    }

    public void SetValue(string? name, object value) {
        RealKey.SetValue(name, value);
    }

    public void SetValue(string? name, object value, RegistryValueKind valueKind) {
        RealKey.SetValue(name, value, valueKind);
    }
}