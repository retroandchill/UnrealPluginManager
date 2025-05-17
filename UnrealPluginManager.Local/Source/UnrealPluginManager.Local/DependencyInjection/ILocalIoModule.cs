
using System.Text.Json;
using Jab;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Local.Services;

namespace UnrealPluginManager.Local.DependencyInjection;

/// <summary>
/// Represents a service module that provides functionality for local Input/Output operations,
/// including JSON serialization and local storage management.
/// </summary>
[ServiceProviderModule]
[Singleton<IJsonService>(Factory = nameof(CreateJsonService))]
[Singleton<IStorageService, LocalStorageService>]
public interface ILocalIoModule {
  
  /// <summary>
  /// Creates and returns an instance of JsonService configured with specific serialization options.
  /// The options include camel case property naming, camel case dictionary key naming, and indented output.
  /// </summary>
  /// <returns>
  /// An instance of <see cref="JsonService"/> pre-configured with JSON serializer options for camel case naming
  /// and indented formatting.
  /// </returns>
  static JsonService CreateJsonService() {
    var jsonSerializationOptions = new JsonSerializerOptions {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    return new JsonService(jsonSerializationOptions);
  }
  
}