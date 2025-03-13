using System.Text.Json;
using UnrealPluginManager.Core.Converters;

namespace UnrealPluginManager.Core.Tests.Converters;

public class StringKeyDictionaryConverterTest {
  private static readonly JsonSerializerOptions Options = new() {
      DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
      Converters = { new StringKeyDictionaryConverter<int>() }
  };
  
  [Test]
  public void TestSerializeDeserializedDictionary() {
    var dict = new Dictionary<string, int> {
        ["ItemA"] = 3,
        ["ItemB"] = 5
    };
    
    var serialized = JsonSerializer.Serialize(dict, Options);
    var deserialized = JsonSerializer.Deserialize<Dictionary<string, int>>(serialized, Options);
    Assert.That(deserialized, Is.EqualTo(dict));
  }
}