namespace UnrealPluginManager.Core.Abstractions;

/// <summary>
/// Provides methods for interacting with system environment variables and special folder paths.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IEnvironment"/> interface to access environment-related values using methods like
/// <c>GetEnvironmentVariable</c> and <c>GetFolderPath</c>.
/// </remarks>
public class SystemEnvironment : IEnvironment {
    /// <inheritdoc />
    public string? GetEnvironmentVariable(string variable) {
        return Environment.GetEnvironmentVariable(variable);
    }

    /// <inheritdoc />
    public string GetFolderPath(Environment.SpecialFolder folder) {
        return Environment.GetFolderPath(folder);
    }
}