using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Writers;
using Semver;
using Swashbuckle.AspNetCore.Swagger;
using UnrealPluginManager.ApiGenerator.Utils;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Server.Controllers;
using UnrealPluginManager.Server.Utils;

namespace UnrealPluginManager.ApiGenerator.Swagger;

public static class SwaggerExtensions {
    private const string SemVerPattern =
        @"^(?:0|[1-9]\d*)\.(?:0|[1-9]\d*)\.(?:0|[1-9]\d*)(?:-(?:(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?:[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$";
    
    public static WebApplicationBuilder SetUpSwagger(this WebApplicationBuilder builder) {
        builder.Services.AddSingleton<ISwaggerService, SwaggerService>()
            .AddOpenApi()
            .AddEndpointsApiExplorer()
            .AddSwaggerGen()
            .AddSwaggerGen(options => {
            // include API xml documentation
            var apiAssembly = typeof(PluginsController).Assembly;
            options.IncludeXmlComments(GetXmlDocumentationFileFor(apiAssembly));

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
}