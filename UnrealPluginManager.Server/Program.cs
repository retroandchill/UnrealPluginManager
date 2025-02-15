using System.IO.Abstractions;
using System.Reflection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Server.Binding;
using UnrealPluginManager.Server.Database;
using UnrealPluginManager.Server.Services;
using UnrealPluginManager.Server.Swagger;
using UnrealPluginManager.Server.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApiConfigs()
    .AddSystemAbstractions()
    .AddServiceConfigs()
    .AddDbContext<UnrealPluginManagerContext, CloudUnrealPluginManagerContext>()
    .AddCoreServices()
    .AddServerServices()
    .AddControllers(options => {
        options.ModelBinderProviders.Insert(0, new PaginationModelBinderProvider());
    });

builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = null);

if (builder.Environment.IsDevelopment()) {
    builder.Services.AddSwaggerGen(options => {
        options.MapType<SemVersion>(() => new OpenApiSchema {
            Type = "string",
            Pattern =
                @"^(?:0|[1-9]\d*)\.(?:0|[1-9]\d*)\.(?:0|[1-9]\d*)(?:-(?:(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*)(?:\.(?:0|[1-9]\d*|\d*[a-zA-Z-][0-9a-zA-Z-]*))*))?(?:\+(?:[0-9a-zA-Z-]+(?:\.[0-9a-zA-Z-]+)*))?$",
            Example = new OpenApiString("1.0.0")
        });
        options.MapType<SemVersionRange>(() => new OpenApiSchema {
            Type = "string",
            Example = new OpenApiString(">=1.0.0")
        });
        options.AddSchemaFilterInstance(new CollectionPropertyFilter());
    });
}

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

await app.RunAsync();