using System.Text.Json;
using System.Text.Json.Serialization;
using Semver;

namespace UnrealPluginManager.Core.Converters;

public class SemVersionJsonConverter : JsonConverter<SemVersion> {
    /// <inheritdoc/>
    public override SemVersion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        return SemVersion.Parse(reader.GetString()!);
    }


    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, SemVersion value, JsonSerializerOptions options) {
        writer.WriteStringValue(value.ToString());
    }
}