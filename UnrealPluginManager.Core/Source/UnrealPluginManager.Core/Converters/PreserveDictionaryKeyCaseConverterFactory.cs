using System.Text.Json;
using System.Text.Json.Serialization;

namespace UnrealPluginManager.Core.Converters;

public class PreserveDictionaryKeyCaseConverterFactory : JsonConverterFactory {

  public override bool CanConvert(Type typeToConvert) {
    return typeToConvert.IsGenericType &&
           typeToConvert.GetGenericTypeDefinition() == typeof(Dictionary<,>) &&
           typeToConvert.GetGenericArguments()[0] == typeof(string); // Apply only to string-keyed dictionaries
  }


  public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
    // Create the generic converter for the Dictionary
    var keyType = typeToConvert.GetGenericArguments()[0];
    var valueType = typeToConvert.GetGenericArguments()[1];

    var converterType = typeof(StringKeyDictionaryConverter<>).MakeGenericType(valueType);
    return (JsonConverter)Activator.CreateInstance(converterType);

  }
}