using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using UnrealPluginManager.Core.Pagination;

namespace UnrealPluginManager.Core.Converters;

/// <summary>
/// A JSON converter for the <see cref="Page{T}"/> class that processes paginated data during serialization and deserialization.
/// </summary>
/// <typeparam name="T">The type of items contained within the paginated collection.</typeparam>
/// <remarks>
/// The <c>PageJsonConverter</c> handles the conversion of <see cref="Page{T}"/> objects to their JSON representation and vice versa.
/// During deserialization, it expects a JSON object containing the keys "pageNumber", "pageSize", and "items". Any missing keys will result in a <see cref="JsonException"/>.
/// During serialization, the <see cref="Page{T}"/> object is written as a JSON object with the following keys and values:
/// - "pageNumber": the current page number.
/// - "totalPages": the total number of pages.
/// - "pageSize": the size of the page.
/// - "count": the total count of items.
/// - "items": the collection of items in the current page.
/// </remarks>
public class PageJsonConverter<T> : JsonConverter<Page<T>> {

    /// <inheritdoc/>
    public override Page<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        using var document = JsonDocument.ParseValue(ref reader);
        var jsonNode = JsonNode.Parse(document.RootElement.GetRawText());
        ArgumentNullException.ThrowIfNull(jsonNode);
        var obj = jsonNode.AsObject();
        var pageNumber = obj["pageNumber"]?.GetValue<int>();
        var pageSize = obj["pageSize"]?.GetValue<int>();
        var items = JsonSerializer.Deserialize<List<T>>(obj["items"]!.ToString(), options);
        if (pageNumber == null || pageSize == null || items == null) {
            throw new JsonException("Invalid page object.");
        }
        
        return new Page<T>(items, items.Count, pageNumber.Value, pageSize.Value);
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Page<T> value, JsonSerializerOptions options) {
        writer.WriteStartObject();
        writer.WriteNumber("pageNumber", value.PageNumber);
        writer.WriteNumber("totalPages", value.TotalPages);
        writer.WriteNumber("pageSize", value.PageSize);
        writer.WriteNumber("count", value.Count);
        writer.WriteStartArray("items");
        foreach (var item in value) {
            JsonSerializer.Serialize(writer, item, options);
        }
        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}