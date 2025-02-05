using System.Text.Json;
using System.Text.Json.Serialization;
using Semver;

namespace UnrealPluginManager.Core.Converters;

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