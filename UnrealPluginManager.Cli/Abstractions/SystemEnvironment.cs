namespace UnrealPluginManager.Cli.Abstractions;

public class SystemEnvironment : IEnvironment {
    public string? GetEnvironmentVariable(string variable) {
        return Environment.GetEnvironmentVariable(variable);
    }

    public string GetFolderPath(Environment.SpecialFolder folder) {
        return Environment.GetFolderPath(folder);
    }
}