namespace UnrealPluginManager.Server.Model.Files;

/// <summary>
/// Represents a submission of an Unreal Plugin, including the source code and associated binary files.
/// </summary>
/// <remarks>
/// This class is primarily used to handle data submitted for plugin creation or updates via the API.
/// </remarks>
public class PluginSubmission {
  /// <summary>
  /// Gets or sets the source code file submitted as part of the plugin submission.
  /// </summary>
  /// <remarks>
  /// This property represents the primary code file for the Unreal plugin being submitted.
  /// It is expected to be provided as part of a multipart form-data request.
  /// </remarks>
  public required IFormFile SourceCode { get; set; }

  /// <summary>
  /// Gets or sets the binary files associated with the plugin submission, organized by Unreal Engine version and platform.
  /// </summary>
  /// <remarks>
  /// This property contains binary files for multiple versions of Unreal Engine and their specific platforms.
  /// Each version is associated with a dictionary where the key represents the platform (e.g., Windows, Linux)
  /// and the value is the file submitted for that platform.
  /// </remarks>
  public required Dictionary<Version, Dictionary<string, IFormFile>> Binaries { get; set; }
  
}