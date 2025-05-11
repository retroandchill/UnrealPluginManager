namespace UnrealPluginManager.Core.Model.Plugins.Recipes;

/// <summary>
/// Represents information about a source patch file, including its filename
/// and content.
/// </summary>
/// <remarks>
/// This struct is used to encapsulate the filename and content of a patch file,
/// primarily for plugins in the Unreal Plugin Manager system. It may be used
/// in conjunction with asynchronous service methods for retrieving patch data.
/// </remarks>
public record struct SourcePatchInfo(string Filename, string Content);