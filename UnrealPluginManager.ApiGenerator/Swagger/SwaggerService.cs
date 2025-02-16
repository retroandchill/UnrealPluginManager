using System.IO.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Writers;
using Swashbuckle.AspNetCore.Swagger;

namespace UnrealPluginManager.ApiGenerator.Swagger;

public class SwaggerService(ISwaggerProvider swaggerProvider, IFileSystem fileSystem) : ISwaggerService {

    public async Task GenerateSwaggerAsync(string schemaName, string destination) {
        var swagger = swaggerProvider.GetSwagger(schemaName);
        await using var writer = fileSystem.File.CreateText(destination);
        swagger.SerializeAsV3(new OpenApiJsonWriter(writer));
        Console.WriteLine($"Swagger file generated at {Path.GetFullPath(destination)}");
    }
}