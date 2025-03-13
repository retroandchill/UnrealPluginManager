using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnrealPluginManager.Core.Converters;

public class StringKeyDictionaryConverter<TValue> : JsonConverter<Dictionary<string, TValue>>
{
  public override Dictionary<string, TValue>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
  {
    var result = new Dictionary<string, TValue>();

    if (reader.TokenType != JsonTokenType.StartObject)
      throw new JsonException();

    while (reader.Read())
    {
      if (reader.TokenType == JsonTokenType.EndObject)
        return result;

      if (reader.TokenType != JsonTokenType.PropertyName)
        throw new JsonException();

      string key = reader.GetString(); // Keep the casing of the dictionary key
      reader.Read();
      TValue value = JsonSerializer.Deserialize<TValue>(ref reader, options)!;
      result[key] = value;
    }

    throw new JsonException();
  }

  public override void Write(Utf8JsonWriter writer, Dictionary<string, TValue> value, JsonSerializerOptions options)
  {
    writer.WriteStartObject();

    foreach (var kvp in value)
    {
      writer.WritePropertyName(kvp.Key); // Write the key exactly as it is
      JsonSerializer.Serialize(writer, kvp.Value, options);
    }

    writer.WriteEndObject();
  }
}