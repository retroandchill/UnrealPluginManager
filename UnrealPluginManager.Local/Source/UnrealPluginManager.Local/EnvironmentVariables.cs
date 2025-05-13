namespace UnrealPluginManager.Local;

/// <summary>
/// Provides constants for environment variables used within the Unreal Plugin Manager.
/// </summary>
/// <remarks>
/// These environment variables are used to configure and manage specific behaviors of the Unreal Plugin Manager,
/// such as specifying the primary Unreal Engine version to target or defining the storage directory for local data.
/// </remarks>
public static class EnvironmentVariables {
  /// <summary>
  /// Represents the environment variable name used to specify the primary Unreal Engine version targeted by the Unreal Plugin Manager.
  /// </summary>
  /// <remarks>
  /// This variable is used to configure which Unreal Engine version is considered primary when determining
  /// engine-specific operations, such as displaying the current version or identifying default project compatibility.
  /// The value of this variable should be set to match the version name as recognized by the installed engines.
  /// </remarks>
  public const string PrimaryUnrealEngineVersion = "UPM_PRIMARY_UNREAL_ENGINE_VERSION";

  /// <summary>
  /// Represents the environment variable name used to specify the custom storage directory for Unreal Plugin Manager.
  /// </summary>
  /// <remarks>
  /// This variable determines the location where the Unreal Plugin Manager stores its data.
  /// If this environment variable is set, its value is used as the base directory for storage operations.
  /// Otherwise, a default directory under the user's profile folder is used.
  /// </remarks>
  public const string StorageDirectory = "UPM_STORAGE_DIRECTORY";
}