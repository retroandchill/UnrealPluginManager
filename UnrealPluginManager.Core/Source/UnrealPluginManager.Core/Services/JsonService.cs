using System.Text.Json;
using Retro.ReadOnlyParams.Annotations;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Provides JSON serialization and deserialization services.
/// </summary>
public class JsonService([ReadOnly] JsonSerializerOptions options) : IJsonService {
  /// <inheritdoc />
  public string Serialize(object? obj) {
    return JsonSerializer.Serialize(obj, options);
  }

  /// <inheritdoc />
  public T Deserialize<T>(string json) {
    var deserialized = JsonSerializer.Deserialize<T>(json, options);
    ArgumentNullException.ThrowIfNull(deserialized);
    return deserialized;
  }

  /// <inheritdoc />
  public async ValueTask<T> DeserializeAsync<T>(Stream stream) {
    var deserialized = await JsonSerializer.DeserializeAsync<T>(stream, options);
    ArgumentNullException.ThrowIfNull(deserialized);
    return deserialized;
  }
}