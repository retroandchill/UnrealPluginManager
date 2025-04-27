using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Converters;

/// <summary>
/// A custom JSON converter for handling serialization and deserialization
/// of <see cref="DateTimeOffset"/> objects.
/// </summary>
/// <remarks>
/// This converter is designed to parse <see cref="DateTimeOffset"/> values from strings
/// during deserialization and to write <see cref="DateTimeOffset"/> values as strings
/// during serialization.
/// </remarks>
public class DateTimeOffsetJsonConverter : JsonConverter<DateTimeOffset> {
  /// <inheritdoc />
  public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
    Debug.Assert(typeToConvert == typeof(DateTimeOffset));
    return DateTimeOffset.Parse(reader.GetString().RequireNonNull(), new CultureInfo("en-US"));
  }

  /// <inheritdoc />
  public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options) {
    writer.WriteStringValue(value.ToString());
  }
}