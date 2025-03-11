namespace UnrealPluginManager.Core.Model.Engine;

/// <summary>
/// Represents the binary metadata of a plugin, detailing the Unreal Engine version and platform it aligns with.
/// </summary>
/// <remarks>
/// The purpose of this type is to categorize plugin binaries based on their compatibility with specific engine versions and platform requirements.
/// </remarks>
public record struct PluginBinaryType(string EngineVersion, string Platform);