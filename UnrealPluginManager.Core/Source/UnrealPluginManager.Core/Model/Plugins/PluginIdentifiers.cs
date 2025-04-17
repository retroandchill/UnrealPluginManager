namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents the identifiers for a plugin, including a globally unique identifier (GUID) and its corresponding name.
/// </summary>
/// <param name="Id">The globally unique identifier (GUID) of the plugin.</param>
/// <param name="Name">The name of the plugin.</param>
public record struct PluginIdentifiers(Guid Id, string Name = "");