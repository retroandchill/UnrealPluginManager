using System.Numerics;
using Microsoft.OpenApi.Models;
using Semver;
using Swashbuckle.AspNetCore.SwaggerGen;
using UnrealPluginManager.Core.Model.Plugins.Recipes;

namespace UnrealPluginManager.ApiGenerator.Swagger;

public class AdditionalSchemaDocumentFilter : IDocumentFilter {
  public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context) {
    context.SchemaGenerator.GenerateSchema(
        typeof(PluginManifest),
        context.SchemaRepository);
    context.SchemaRepository.Schemas.Remove(nameof(BigInteger));
    context.SchemaRepository.Schemas.Remove(nameof(MetadataIdentifier));
    context.SchemaRepository.Schemas.Remove(nameof(PrereleaseIdentifier));
  }
}