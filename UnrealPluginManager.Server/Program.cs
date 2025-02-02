using Microsoft.EntityFrameworkCore;
using UnrealPluginManager.Core.Database;
using UnrealPluginManager.Core.Services;
using UnrealPluginManager.Server.Swagger;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<UnrealPluginManagerContext>(options => options.UseSqlite("Filename=dev.sqlite", 
    b => b.MigrationsAssembly("UnrealPluginManager.Server")));
builder.Services.AddScoped<IPluginService, PluginService>();

if (builder.Environment.IsDevelopment()) {
    builder.Services.AddSwaggerGen(options => {
        options.AddSchemaFilterInstance(new CollectionPropertyFilter());
    });
}

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var pluginService = scope.ServiceProvider.GetRequiredService<IPluginService>();
    pluginService.ImportEnginePlugins(@"C:\Program Files\Epic Games\UE_5.5\Engine\Plugins");
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

await app.RunAsync();
