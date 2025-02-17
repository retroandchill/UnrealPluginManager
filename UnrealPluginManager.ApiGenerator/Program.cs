// See https://aka.ms/new-console-template for more information

using Microsoft.AspNetCore.Builder;
using UnrealPluginManager.ApiGenerator.Swagger;
using UnrealPluginManager.ApiGenerator.Utils;
using UnrealPluginManager.Server.Utils;

await WebApplication.CreateBuilder(args)
    .SetUpProductionApplication()
    .SetUpSwagger()
    .ConfigureEndpoints()
    .Build()
    .Configure()
    .ProduceSwaggerDocument();



