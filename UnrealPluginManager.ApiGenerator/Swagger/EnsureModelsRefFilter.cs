using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using UnrealPluginManager.Core.Model.Resolution;

namespace UnrealPluginManager.ApiGenerator.Swagger;

/// <summary>
/// A schema filter that ensures specific models are not inlined by enforcing
/// their reference to OpenAPI schema components.
/// </summary>
/// <remarks>
/// This filter is particularly designed to handle `ResolutionResult` and its subtypes.
/// It explicitly modifies the schema to create a `$ref` to a predefined component,
/// removing inlining by altering schema properties such as `Type` and `Properties`.
/// </remarks>
/// <example>
/// To use this filter, ensure it is added during Swagger generation configuration
/// using `SwaggerGenOptions.AddSchemaFilterInstance`.
/// </example>
public class EnsureModelsRefFilter : ISchemaFilter {
  
  /// <inheritdoc />
  public void Apply(OpenApiSchema schema, SchemaFilterContext context) {
    // Ensure `ResolutionResult` and its subtypes are not inlined
    if (context.Type != typeof(ResolutionResult) || schema.OneOf.Count == 0) return;
    
    // Prevent inline schema by explicitly setting this schema as a component reference
    schema.Reference = new OpenApiReference {
        Type = ReferenceType.Schema,
        Id = context.Type.Name
    };

    // Clear inlining properties to enforce `$ref`
    schema.Properties = null;
    schema.Type = null;
  }
}