using System.Text.Json;
using System.Text.Json.Serialization;
using Semver;

namespace UnrealPluginManager.Core.Converters;

/// <summary>
/// A custom JSON converter for handling serialization and deserialization of <see cref="SemVersion"/> objects.
/// </summary>
/// <remarks>
/// This class enables seamless JSON serialization and deserialization of semantic versioning objects using the
/// Semver library. It ensures that <see cref="SemVersion"/> instances are converted to and from JSON string
/// representations in a consistent manner.
/// </remarks>
/// <seealso cref="SemVersion"/>
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