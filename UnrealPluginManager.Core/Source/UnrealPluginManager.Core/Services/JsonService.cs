using System.Text.Json;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Provides JSON serialization and deserialization services.
/// </summary>
[AutoConstructor]
public partial class JsonService : IJsonService {
  private readonly JsonSerializerOptions _options;

  /// <inheritdoc />
  public string Serialize(object? obj) {
    return JsonSerializer.Serialize(obj, _options);
  }

  /// <inheritdoc />
  public T Deserialize<T>(string json) {
    var deserialized = JsonSerializer.Deserialize<T>(json, _options);
    ArgumentNullException.ThrowIfNull(deserialized);
    return deserialized;
  }

  /// <inheritdoc />
  public async ValueTask<T> DeserializeAsync<T>(Stream stream) {
    var deserialized = await JsonSerializer.DeserializeAsync<T>(stream, _options);
    ArgumentNullException.ThrowIfNull(deserialized);
    return deserialized;
  }
}