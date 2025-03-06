using System.Reflection;
using Dunet;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UnrealPluginManager.ApiGenerator.Swagger;

/// <summary>
/// A custom schema filter that processes types marked with a union attribute
/// and modifies the OpenAPI schema definitions for discriminated unions.
/// </summary>
/// <remarks>
/// This schema filter dynamically updates the OpenAPI schema to account for
/// discriminated unions by defining the `oneOf` property for subclasses of
/// a union type. It adjusts schema properties, such as removing redundancies
/// and ensuring proper type identification through the union discriminator.
/// </remarks>
/// <seealso cref="ISchemaFilter" />
public class DiscriminatedUnionSchemaFilter : ISchemaFilter {
  /// <inheritdoc />
  public void Apply(OpenApiSchema schema, SchemaFilterContext context) {
    if (context.Type.GetCustomAttribute<UnionAttribute>() is not null) {
      schema.OneOf = context.Type.GetNestedTypes()
          .Where(t => t.IsSubclassOf(context.Type))
          .Select(t => {
            var res = context.SchemaRepository.TryLookupByType(t, out var schemaRef);
            return (Success: res, Schema: schemaRef);
          })
          .Where(t => t.Success)
          .Select(t => t.Schema.Reference.Id)
          .Select(s => new OpenApiSchema {
              Reference = new OpenApiReference {
                  Id = s,
                  Type = ReferenceType.Schema
              }
          })
          .ToList();

      schema.Required = null;
      schema.Properties = null;
      schema.AdditionalProperties = null;
      return;
    }

    var superclass = context.Type.BaseType;
    if (superclass?.GetCustomAttribute<UnionAttribute>() is null) {
      return;
    }
    
    var inner = schema.AllOf.LastOrDefault();
    if (inner is null) {
      return;
    }
        
    inner.Description = schema.Description;
    inner.Properties.Add("type", new OpenApiSchema {
        Type = "string",
        Nullable = false,
        MinLength = 1
    });
    inner.Required.Add("type");
    
    schema.Properties = inner.Properties;
    schema.Required = inner.Required;
    schema.AllOf = null;
    schema.Type = inner.Type;
    schema.AdditionalProperties = inner.AdditionalProperties;
  }
}