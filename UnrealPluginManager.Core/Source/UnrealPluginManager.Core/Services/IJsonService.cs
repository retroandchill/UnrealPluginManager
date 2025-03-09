namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Provides methods for JSON serialization and deserialization.
/// </summary>
public interface IJsonService {

  /// <summary>
  /// Serializes the specified object to a JSON string.
  /// </summary>
  /// <param name="obj">The object to serialize. Can be null.</param>
  /// <returns>A JSON string representation of the object.</returns>
  string Serialize(object? obj);

  /// <summary>
  /// Deserializes the specified JSON string into an object of the specified type.
  /// </summary>
  /// <param name="json">The JSON string to deserialize. Must not be null or empty.</param>
  /// <typeparam name="T">The type into which the JSON string is deserialized.</typeparam>
  /// <returns>An object of type T that represents the deserialized JSON string.</returns>
  T Deserialize<T>(string json);

  /// <summary>
  /// Deserializes JSON data from the specified stream asynchronously into an object of type <typeparamref name="T"/>.
  /// </summary>
  /// <typeparam name="T">The type of object to deserialize the JSON data into.</typeparam>
  /// <param name="stream">The stream containing JSON data to deserialize.</param>
  /// <returns>An object of type <typeparamref name="T"/> deserialized from the JSON data.</returns>
  ValueTask<T> DeserializeAsync<T>(Stream stream);
  
}