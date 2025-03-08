$current_directory = Get-Location
try {
    Set-Location UnrealPluginManager.Server/Source/UnrealPluginManager.Server
    dotnet ef migrations add $args
    dotnet ef database update

    Set-Location ../../../UnrealPluginManager.Local/Source/UnrealPluginManager.Cli
    dotnet ef migrations add $args --project ../UnrealPluginManager.Local
    dotnet ef database update --project ../UnrealPluginManager.Local
} finally {
    Set-Location $current_directory
}