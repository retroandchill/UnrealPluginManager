using UnrealPluginManager.Server.Utils;

await WebApplication.CreateBuilder(args)
    .SetUpProductApplication()
    .Build()
    .Configure()
    .RunAsync();