using UnrealPluginManager.Core.Abstractions;

namespace UnrealPluginManager.Core.Tests.Mocks;

/// <summary>
/// Provides a mock implementation of the <see cref="IEnvironment"/> interface for testing purposes.
/// </summary>
public class MockEnvironment : IEnvironment {

  /// <summary>
  /// Represents a collection of key-value pairs that define environment variables for the application.
  /// </summary>
  /// <remarks>
  /// This property is intended to simulate and manage environment variables for testing purposes,
  /// allowing for controlled test scenarios by providing an injectable mock implementation.
  /// </remarks>
  public Dictionary<string, string> EnvironmentVariables { get; } = new();
  /// <summary>
  /// Represents a collection of mappings between <see cref="Environment.SpecialFolder"/>
  /// values and their corresponding folder path strings for testing purposes.
  /// </summary>
  /// <remarks>
  /// This property allows for simulating and controlling folder paths typically provided by
  /// the environment, enabling mock implementations to override system defaults during tests.
  /// The mappings are used when retrieving folder paths for specific <see cref="Environment.SpecialFolder"/> enums.
  /// </remarks>
  public Dictionary<Environment.SpecialFolder, string> SpecialFolders { get; } = new();

  /// <inheritdoc />
  public string? GetEnvironmentVariable(string variable) {
    return EnvironmentVariables.GetValueOrDefault(variable);
  }

  /// <inheritdoc />
  public string GetFolderPath(Environment.SpecialFolder folder) {
    var result = SpecialFolders.GetValueOrDefault(folder);
    Assert.That(result, Is.Not.Null);
    return result;
  }
}