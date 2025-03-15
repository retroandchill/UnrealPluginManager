using System.Runtime.Versioning;
using UnrealPluginManager.Core.Abstractions;

namespace UnrealPluginManager.Core.Tests.Mocks;

/// <summary>
/// A mock implementation of the <see cref="IRegistryKey"/> interface used for simulating
/// registry access during testing scenarios.
/// </summary>
/// <remarks>
/// This class provides a virtualized representation of registry keys and their associated values
/// without interacting with the actual Windows registry. It allows for unit testing scenarios
/// that involve registry operations while ensuring the safety and isolation of the test environment.
/// The <see cref="MockRegistryKey"/> class stores subkeys and values in-memory using dictionaries
/// and provides methods for accessing and querying these properties, mimicking real registry
/// behavior.
/// </remarks>
[SupportedOSPlatform("windows")]
public class MockRegistryKey : IRegistryKey {
  /// <summary>
  /// Gets or sets the collection of subkeys under the current registry key.
  /// </summary>
  /// <remarks>
  /// Provides access to a dictionary where keys are the names of subkeys and values
  /// are instances of <see cref="IRegistryKey"/> representing each subkey.
  /// This property is used to simulate the hierarchical structure of registry keys
  /// in a virtualized environment for testing purposes.
  /// </remarks>
  public Dictionary<string, IRegistryKey> SubKeys { get; set; } = new();

  /// <summary>
  /// Gets or sets the collection of values associated with the current registry key.
  /// </summary>
  /// <remarks>
  /// Provides access to a dictionary where keys are the names of the values and values
  /// are the corresponding data stored in the registry key. This property simulates
  /// the functionality of storing and retrieving registry values in a virtualized environment,
  /// typically for testing scenarios without modifying the actual Windows registry.
  /// </remarks>
  public Dictionary<string, object> Values { get; set; } = new();

  /// <inheritdoc />
  public IRegistryKey? OpenSubKey(string name) {
    var splitString = name.Split('\\', 2);
    if (splitString.Length < 2) {
      return SubKeys
          .Where(x => x.Key.Equals(splitString[0], StringComparison.OrdinalIgnoreCase))
          .Select(x => x.Value)
          .FirstOrDefault();
    }

    var key = splitString[0];
    var remainder = splitString[1];
    var match = SubKeys
        .Where(x => x.Key.Equals(key, StringComparison.OrdinalIgnoreCase))
        .Select(x => x.Value)
        .FirstOrDefault();
    return match?.OpenSubKey(remainder);
  }

  /// <inheritdoc />
  public string[] GetSubKeyNames() {
    return SubKeys.Keys.ToArray();
  }

  /// <inheritdoc />
  public string[] GetValueNames() {
    return Values.Keys.ToArray();
  }

  /// <inheritdoc />
  public T? GetValue<T>(string name) {
    return (T?)Values.GetValueOrDefault(name);
  }
}