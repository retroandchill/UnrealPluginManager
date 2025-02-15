using System.Text.Json;
using System.Text.Json.Serialization;
using UnrealPluginManager.Core.Pagination;

namespace UnrealPluginManager.Core.Converters;

/// <summary>
/// Provides a custom JSON conversion for the <see cref="Page{T}"/> class.
/// </summary>
/// <typeparam name="T">The type of the elements in the paginated collection.</typeparam>
/// <remarks>
/// This converter serializes a <see cref="Page{T}"/> object into a JSON object with the following structure:
/// - "pageNumber": The number of the current page.
/// - "totalPages": The total number of pages.
/// - "count": The number of items in the current page.
/// - "items": An array containing the serialized items of type <typeparamref name="T"/>.
/// The converter de-serialization is not supported and will throw a <see cref="NotSupportedException"/>.
/// </remarks>
public class PageJsonConverter<T> : JsonConverter<Page<T>> {

    /// <inheritdoc/>
    public override Page<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Page<T> value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WriteNumber("pageNumber", value.PageNumber);
        writer.WriteNumber("totalPages", value.TotalPages);
        writer.WriteNumber("count", value.Count);
        writer.WriteStartArray("items");
        foreach (var item in value) {
            JsonSerializer.Serialize(writer, item, options);
        }
        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}