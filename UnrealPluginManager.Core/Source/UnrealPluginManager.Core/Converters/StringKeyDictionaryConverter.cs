using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnrealPluginManager.Core.Converters;

/// <summary>
/// A custom JsonConverter that facilitates the serialization and deserialization of a dictionary
/// with string keys and values of type <typeparamref name="TValue"/>.
/// </summary>
/// <typeparam name="TValue">
/// The type of the values in the dictionary being serialized or deserialized.
/// </typeparam>
/// <remarks>
/// This converter ensures that dictionary keys are preserved exactly as they are during
/// the serialization and deserialization process (e.g., casing is not modified). It handles
/// JSON objects where each property corresponds to a key-value pair in the dictionary.
/// </remarks>
public class StringKeyDictionaryConverter<TValue> : JsonConverter<Dictionary<string, TValue>> {
  /// <inheritdoc />
  public override Dictionary<string, TValue> Read(ref Utf8JsonReader reader, Type typeToConvert,
                                                  JsonSerializerOptions options) {
    var result = new Dictionary<string, TValue>();
    
    if (reader.TokenType != JsonTokenType.StartObject) {
      throw new JsonException();
    }

    while (reader.Read()) {
      if (reader.TokenType == JsonTokenType.EndObject)
        return result;

      if (reader.TokenType != JsonTokenType.PropertyName)
        throw new JsonException();

      var key = reader.GetString(); // Keep the casing of the dictionary key
      ArgumentNullException.ThrowIfNull(key);
      reader.Read();
      var value = JsonSerializer.Deserialize<TValue>(ref reader, options)!;
      result[key] = value;
    }

    throw new JsonException();
  }

  /// <inheritdoc />
  public override void Write(Utf8JsonWriter writer, Dictionary<string, TValue> value, JsonSerializerOptions options) {
    writer.WriteStartObject();

    foreach (var kvp in value) {
      writer.WritePropertyName(kvp.Key); // Write the key exactly as it is
      JsonSerializer.Serialize(writer, kvp.Value, options);
    }

    writer.WriteEndObject();
  }
}