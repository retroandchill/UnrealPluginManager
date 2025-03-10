namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents information about a retrieved binary for a plugin, including its name, version, and supported platforms.
/// </summary>
public record RetrievedBinaryInformation(string Name, string Version, List<string> Platforms);