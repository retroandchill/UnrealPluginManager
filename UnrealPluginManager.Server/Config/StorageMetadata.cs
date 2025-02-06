namespace UnrealPluginManager.Server.Config;

public class StorageMetadata {
    
    public const string Name = "CloudStorage";

    
    private string _baseDirectory = Directory.GetCurrentDirectory();
    public string BaseDirectory {
        get => _baseDirectory;
        set => _baseDirectory = Path.GetFullPath(value, Directory.GetCurrentDirectory());
    }
}