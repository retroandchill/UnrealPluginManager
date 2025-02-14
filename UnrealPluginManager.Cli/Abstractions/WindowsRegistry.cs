using System.Runtime.Versioning;

namespace UnrealPluginManager.Cli.Abstractions;

/// <summary>
/// Provides an implementation of the <see cref="IRegistry"/> interface for accessing the Windows registry.
/// </summary>
/// <remarks>
/// This class is supported only on Windows platforms. It provides access to the LocalMachine and CurrentUser
/// root registry hives through the properties <see cref="LocalMachine"/> and <see cref="CurrentUser"/> respectively.
/// </remarks>
[SupportedOSPlatform("windows")]
public class WindowsRegistry : IRegistry {
    /// <inheritdoc />
    public IRegistryKey LocalMachine => new WindowsRegistryKey(Microsoft.Win32.Registry.LocalMachine);

    /// <inheritdoc />
    public IRegistryKey CurrentUser => new WindowsRegistryKey(Microsoft.Win32.Registry.CurrentUser);
}