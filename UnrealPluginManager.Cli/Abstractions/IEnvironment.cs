namespace UnrealPluginManager.Cli.Abstractions;

public interface IEnvironment {
    string? GetEnvironmentVariable(string variable);

    string GetFolderPath(Environment.SpecialFolder folder);
}