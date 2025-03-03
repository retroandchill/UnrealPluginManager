using System.Runtime.Versioning;
using UnrealPluginManager.Core.Abstractions;

namespace UnrealPluginManager.Core.Tests.Mocks;

/// <summary>
/// Provides utility methods for interacting with mock implementations of the Windows registry.
/// </summary>
[SupportedOSPlatform("windows")]
public static class MockRegistryUtils {
  /// <summary>
  /// Opens an existing subkey by the specified name or creates it if it does not exist.
  /// </summary>
  /// <param name="key">The current registry key represented by <see cref="IRegistryKey"/>.</param>
  /// <param name="name">The relative path or name of the subkey to open or create.</param>
  /// <returns>A <see cref="MockRegistryKey"/> instance representing the subkey that was opened or created.</returns>
  /// <exception cref="ArgumentException">Thrown if the provided key is not a <see cref="MockRegistryKey"/>.</exception>
  public static MockRegistryKey OpenOrAddSubKey(this IRegistryKey key, string name) {
    var castedKey = (MockRegistryKey)key;
    if (castedKey is null) {
      throw new ArgumentException("Key is not a MockRegistryKey");
    }

    IRegistryKey? subkeyValue = null;
    foreach (var subkey in name.Split('\\')) {
      if (!castedKey.SubKeys.TryGetValue(subkey, out subkeyValue)) {
        subkeyValue = new MockRegistryKey();
        castedKey.SubKeys.Add(subkey, subkeyValue);
      }

      castedKey = (MockRegistryKey)subkeyValue;
    }

    return (MockRegistryKey)subkeyValue!;
  }

  /// <summary>
  /// Sets a value in the registry key with the specified name and value.
  /// </summary>
  /// <param name="key">The current registry key represented by <see cref="IRegistryKey"/>.</param>
  /// <param name="name">The name of the value to set.</param>
  /// <param name="value">The value to assign to the registry key.</param>
  /// <exception cref="ArgumentException">Thrown if the provided key is not a <see cref="MockRegistryKey"/>.</exception>
  public static void SetValue(this IRegistryKey key, string name, object value) {
    var castedKey = (MockRegistryKey)key;
    if (castedKey is null) {
      throw new ArgumentException("Key is not a MockRegistryKey");
    }

    castedKey.Values.Add(name, value);
  }
}