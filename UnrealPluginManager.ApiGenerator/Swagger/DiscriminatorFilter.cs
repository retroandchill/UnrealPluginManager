using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using UnrealPluginManager.Core.Model.Resolution;

namespace UnrealPluginManager.ApiGenerator.Swagger;

/// <summary>
/// Represents a schema filter that applies a discriminator property to a schema in the Swagger documentation.
/// </summary>
/// <remarks>
/// This filter is used to define a discriminator on schemas for polymorphic models. It modifies
/// the schema definition by adding a discriminator property and specifying required fields.
/// </remarks>
/// <example>
/// The filter is typically used in Swagger configuration to handle polymorphism and ensure models
/// are correctly defined with a discriminator for OpenAPI documentation purposes.
/// </example>
public class DiscriminatorFilter : ISchemaFilter {
  /// <inheritdoc />
  public void Apply(OpenApiSchema schema, SchemaFilterContext context) {
    if (context.Type != typeof(ResolutionResult)) {
      if (context.Type.IsSubclassOf(typeof(ResolutionResult))) {
        schema.Properties.Remove("type");
      }
      
      return;
    }
    // Add the discriminator property
    schema.Discriminator = new OpenApiDiscriminator
    {
        PropertyName = "type",
        Mapping = new Dictionary<string, string>
        {
            { "Resolved", nameof(ResolvedDependencies) },
            { "ConflictsDetected", nameof(ConflictDetected) }
        }
    };

    // Ensure "type" is listed as required
    if (!schema.Required.Contains("type"))
    {
      schema.Required.Add("type");
    }
  }
}