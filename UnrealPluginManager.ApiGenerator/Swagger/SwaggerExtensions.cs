using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Semver;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using UnrealPluginManager.ApiGenerator.Utils;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Resolution;
using UnrealPluginManager.Server.Controllers;
using UnrealPluginManager.Server.Utils;

namespace UnrealPluginManager.ApiGenerator.Swagger;

/// <summary>
/// Provides extension methods for configuring and generating Swagger documentation in a .NET application.
/// </summary>
public static class SwaggerExtensions {
  /// <summary>
  /// A regular expression pattern for Semantic Versioning (SemVer) that complies with SemVer 2.0.0 specification.
  /// </summary>
  /// <remarks>
  /// This pattern matches version strings in the format of Major.Minor.Patch, optionally followed by
  /// pre-release identifiers (e.g., -alpha, -beta) and build metadata (e.g., +001).
  /// </remarks>
  public const string SemVerPattern =
      @"^(?:0|[1-9]\d*)\.(?:0|[1-9]\d*)\.(?:0|[1-9]\d*)(?:-(?:(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?:[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$";

  /// <summary>
  /// Configures and sets up Swagger for the application, including adding OpenAPI documentation,
  /// XML comments, and various schema and operation filters.
  /// </summary>
  /// <param name="builder">The <see cref="WebApplicationBuilder"/> instance being configured.</param>
  /// <return>
  /// The configured <see cref="WebApplicationBuilder"/> instance with Swagger setup.
  /// </return>
  public static WebApplicationBuilder SetUpSwagger(this WebApplicationBuilder builder) {
    builder.Services.AddSingleton<ISwaggerService, SwaggerService>()
        .AddOpenApi()
        .AddEndpointsApiExplorer()
        .AddSwaggerGen()
        .AddSwaggerGen(options => {
          // include API xml documentation
          options.CustomOperationIds(GetOperationIdName);
          var apiAssembly = typeof(PluginsController).Assembly;
          options.IncludeXmlComments(GetXmlDocumentationFileFor(apiAssembly));
          
          options.EnableAnnotations(true, true);
          
          // include models xml documentation
          var modelsAssembly = typeof(PluginSummary).Assembly;
          options.IncludeXmlComments(GetXmlDocumentationFileFor(modelsAssembly));
          options.MapType<SemVersion>(() => new OpenApiSchema {
              Type = "string",
              Pattern = SemVerPattern,
              Example = new OpenApiString("1.0.0")
          });
          options.MapType<SemVersionRange>(() => new OpenApiSchema {
              Type = "string",
              Example = new OpenApiString(">=1.0.0")
          });
          options.AddSchemaFilterInstance(new CollectionPropertyFilter());
          options.AddOperationFilterInstance(new PageableParameterFilter());
          options.AddSchemaFilterInstance(new PagePropertyFilter());
          options.AddOperationFilterInstance(new SemVersionParameterFilter());
          options.AddSchemaFilterInstance(new DiscriminatorFilter());
          options.AddSchemaFilterInstance(new EnsureModelsRefFilter());
        });

    return builder;
  }

  private static string GetXmlDocumentationFileFor(Assembly assembly) {
    var documentationFile = $"{assembly.GetName().Name}.xml";
    var path = Path.Combine(AppContext.BaseDirectory, documentationFile);

    return path;
  }

  public static Task ProduceSwaggerDocument(this WebApplication app) {
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    var swaggerProvider = app.Services.GetRequiredService<ISwaggerService>();
    return swaggerProvider.GenerateSwaggerAsync("v1", "openapi-spec.json");
  }

  private static string? GetOperationIdName(ApiDescription apiDescription) {
    return apiDescription.TryGetMethodInfo(out var methodInfo)
        ? (char.ToLowerInvariant(methodInfo.Name[0]) + methodInfo.Name[1..])
        : null;
  }
}