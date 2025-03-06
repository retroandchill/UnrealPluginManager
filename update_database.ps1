$current_directory = Get-Location
try {
    Set-Location UnrealPluginManager.Server
    dotnet ef migrations add $args
    dotnet ef database update

    Set-Location ../UnrealPluginManager.Cli
    dotnet ef migrations add $args --project ../UnrealPluginManager.Local
    dotnet ef database update --project ../UnrealPluginManager.Local
} finally {
    Set-Location $current_directory
}