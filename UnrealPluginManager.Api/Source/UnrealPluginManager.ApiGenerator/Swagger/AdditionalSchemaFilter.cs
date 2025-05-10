using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using UnrealPluginManager.Core.Model.Plugins.Recipes;

namespace UnrealPluginManager.ApiGenerator.Swagger;

public class AdditionalSchemaDocumentFilter : IDocumentFilter {
  public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) {
    context.SchemaGenerator.GenerateSchema(
        typeof(PluginManifest),
        context.SchemaRepository);
  }
}