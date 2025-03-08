using UnrealPluginManager.Server.Utils;

await WebApplication.CreateBuilder(args)
    .SetUpProductionApplication()
    .Build()
    .Configure()
    .RunAsync();