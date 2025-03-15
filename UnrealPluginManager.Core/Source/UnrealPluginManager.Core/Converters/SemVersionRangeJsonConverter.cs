using System.Text.Json;
using System.Text.Json.Serialization;
using Semver;

namespace UnrealPluginManager.Core.Converters;

/// <summary>
/// Responsible for converting JSON data to and from the <see cref="SemVersionRange"/> type.
/// This converter is used specifically to handle serialization and deserialization of semantic version ranges
/// as strings in JSON format.
/// </summary>
/// <remarks>
/// In the context of JSON serialization and deserialization, this converter interprets a semantic version range
/// string representation as a <see cref="SemVersionRange"/> object. Similarly, it converts a <see cref="SemVersionRange"/>
/// object back into its string representation when serializing to JSON.
/// </remarks>
public class SemVersionRangeJsonConverter : JsonConverter<SemVersionRange> {
  /// <inheritdoc/>
  public override SemVersionRange Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
    return SemVersionRange.Parse(reader.GetString()!);
  }


  /// <inheritdoc/>
  public override void Write(Utf8JsonWriter writer, SemVersionRange value, JsonSerializerOptions options) {
    writer.WriteStringValue(value.ToString());
  }
}