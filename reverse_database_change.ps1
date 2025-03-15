$current_directory = Get-Location
try {
    Set-Location UnrealPluginManager.Server/Source/UnrealPluginManager.Server
    dotnet ef database update $args
    dotnet ef migrations remove

    Set-Location ../../../UnrealPluginManager.Local/Source/UnrealPluginManager.Cli
    dotnet ef database update $args --project ../UnrealPluginManager.Local
    dotnet ef migrations remove --project ../UnrealPluginManager.Local
} finally {
    Set-Location $current_directory
}