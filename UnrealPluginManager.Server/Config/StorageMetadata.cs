namespace UnrealPluginManager.Server.Config;

/// <summary>
/// Represents metadata configuration for storage settings.
/// </summary>
/// <remarks>
/// This class provides configuration-related properties and constants for storage handling.
/// It is utilized primarily in dependency injection contexts to configure storage services.
/// </remarks>
public class StorageMetadata {

    /// <summary>
    /// A constant string representing the name of the storage configuration section.
    /// </summary>
    /// <remarks>
    /// This field is used to identify and bind the specific configuration settings
    /// related to storage in the application's configuration system.
    /// </remarks>
    public const string Name = "CloudStorage";

    
    private string _baseDirectory = Directory.GetCurrentDirectory();
    
    /// <summary>
    /// Represents the base directory used for storage operations.
    /// </summary>
    /// <remarks>
    /// This property defines the root directory where storage-related files and data
    /// are saved or retrieved. It is resolved relative to the current working directory
    /// if a relative path is provided, and converted to an absolute path accordingly.
    /// </remarks>
    public string BaseDirectory {
        get => _baseDirectory;
        set => _baseDirectory = Path.GetFullPath(value, Directory.GetCurrentDirectory());
    }
}