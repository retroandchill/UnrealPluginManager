using System.Runtime.Versioning;

namespace UnrealPluginManager.Cli.Abstractions;

/// <summary>
/// Represents an abstraction for interacting with a Windows registry key.
/// </summary>
/// <remarks>
/// This interface provides methods to navigate and retrieve data from registry keys.
/// It is platform-specific and only supported on Windows.
/// </remarks>
[SupportedOSPlatform("windows")]
public interface IRegistryKey {
    /// <summary>
    /// Opens a subkey within the current registry key.
    /// </summary>
    /// <param name="name">The name or relative path of the subkey to open.</param>
    /// <returns>
    /// An <see cref="IRegistryKey"/> object representing the opened subkey if it exists;
    /// otherwise, <c>null</c> if the subkey does not exist.
    /// </returns>
    IRegistryKey? OpenSubKey(string name);

    /// <summary>
    /// Retrieves the names of all subkeys within the current registry key.
    /// </summary>
    /// <returns>
    /// An array of strings containing the names of the subkeys in the current registry key.
    /// If there are no subkeys, an empty array is returned.
    /// </returns>
    string[] GetSubKeyNames();

    /// <summary>
    /// Retrieves all value names associated with the current registry key.
    /// </summary>
    /// <returns>
    /// An array of strings representing the names of all values under the current registry key.
    /// </returns>
    string[] GetValueNames();

    /// <summary>
    /// Retrieves the value associated with the specified name from the registry key.
    /// </summary>
    /// <typeparam name="T">The type of the value to be retrieved.</typeparam>
    /// <param name="name">The name of the value to retrieve.</param>
    /// <returns>
    /// The value associated with the specified name, cast to the specified type <typeparamref name="T"/>,
    /// or <c>null</c> if the value does not exist.
    /// </returns>
    T? GetValue<T>(string name);
    
}