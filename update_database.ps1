$current_directory = Get-Location
try {
    Set-Location UnrealPluginManager.Server/Source/UnrealPluginManager.Server
    if ($args.count -gt 0)
    {
        dotnet ef migrations add $args
    }
    dotnet ef database update

    Set-Location ../../../UnrealPluginManager.Local/Source/UnrealPluginManager.Cli
    if ($args.count -gt 0)
    {
        dotnet ef migrations add $args --project ../UnrealPluginManager.Local
    }
    dotnet ef database update --project ../UnrealPluginManager.Local
} finally {
    Set-Location $current_directory
}