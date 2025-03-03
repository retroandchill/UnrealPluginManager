using System.Text.Json;
using System.Text.Json.Serialization;
using UnrealPluginManager.Core.Pagination;

namespace UnrealPluginManager.Core.Converters;

/// <summary>
/// A custom JSON converter factory for handling serialization and deserialization
/// of generic Page collections to and from JSON.
/// </summary>
/// <remarks>
/// This factory is specifically designed to support the <see cref="Page{T}"/> class.
/// It provides the mechanisms to create a typed JSON converter for any generic type
/// that matches the definition of <see cref="Page{T}"/>.
/// </remarks>
public class PageJsonConverterFactory : JsonConverterFactory {
  /// <inheritdoc/>
  public override bool CanConvert(Type typeToConvert) {
    if (!typeToConvert.IsGenericType) {
      return false;
    }

    return typeToConvert.GetGenericTypeDefinition() == typeof(Page<>);
  }

  /// <inheritdoc/>
  public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options) {
    var wrappedType = typeToConvert.GetGenericArguments()[0];

    var converterType = typeof(PageJsonConverter<>).MakeGenericType(wrappedType);
    return (JsonConverter?)Activator.CreateInstance(converterType);
  }
}