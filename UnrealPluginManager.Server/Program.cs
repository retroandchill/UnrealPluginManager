using System.IO.Abstractions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Semver;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Services;
using UnrealPluginManager.Server.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IFileSystem, FileSystem>();
builder.Services.AddDbContext<UnrealPluginManagerContext>(options =>
    options.UseSqlite("Filename=dev.sqlite", b =>
        b.MigrationsAssembly("UnrealPluginManager.Server")
            .MinBatchSize(1)
            .MaxBatchSize(100)));
builder.Services.AddScoped<IPluginService, PluginService>();
builder.Services.AddScoped<IStorageService, CloudStorageService>();
builder.Services.Configure<FormOptions>(options => {
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = long.MaxValue;
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