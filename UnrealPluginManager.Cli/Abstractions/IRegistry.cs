using System.Runtime.Versioning;

namespace UnrealPluginManager.Cli.Abstractions;

/// <summary>
/// Represents a registry abstraction for accessing the Windows registry keys.
/// </summary>
/// <remarks>
/// This interface is platform-specific and only supported on Windows.
/// </remarks>
[SupportedOSPlatform("windows")]
public interface IRegistry {
    /// <summary>
    /// Gets a registry key abstraction representing the HKEY_LOCAL_MACHINE hive of the Windows registry.
    /// </summary>
    /// <remarks>
    /// This property provides access to the Local Machine registry hive, allowing users to read subkeys,
    /// retrieve values, and navigate the registry structure. This is typically used to access system-wide
    /// settings and configurations stored in the Windows registry.
    /// It is only available on Windows and may throw platform-specific exceptions if used on unsupported platforms.
    /// </remarks>
    IRegistryKey LocalMachine { get; }

    /// <summary>
    /// Gets a registry key abstraction representing the HKEY_CURRENT_USER hive of the Windows registry.
    /// </summary>
    /// <remarks>
    /// This property provides access to the Current User registry hive, allowing users to read subkeys,
    /// retrieve values, and navigate the registry structure. This is commonly used to access user-specific
    /// settings and configurations stored in the Windows registry.
    /// It is only available on Windows and may throw platform-specific exceptions if used on unsupported platforms.
    /// </remarks>
    IRegistryKey CurrentUser { get; }
    
}