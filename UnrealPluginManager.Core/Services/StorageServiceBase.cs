using System.IO.Abstractions;
using System.IO.Compression;
using System.Text.Json;
using LanguageExt;
using UnrealPluginManager.Core.Converters;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Storage;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Provides a base implementation for storage services managing plugin files.
/// This abstract class handles common functionality for storage operations such as
/// storing and retrieving plugin files while relying on derived classes for
/// implementation of the base directory logic.
/// </summary>
[AutoConstructor]
public abstract partial class StorageServiceBase : IStorageService {
    
    /// <summary>
    /// Provides access to the file system abstraction used by storage services.
    /// This property is used to perform file system operations without directly
    /// depending on the standard System.IO classes, facilitating unit testing
    /// and custom implementations.
    /// </summary>
    [field: AutoConstructorInject(initializer: "filesystem", 
        injectedType: typeof(IFileSystem), parameterName: "filesystem")]
    protected IFileSystem FileSystem { get; }

    /// <inheritdoc />
    public abstract string BaseDirectory { get; }

    private string PluginDirectory => Path.Join(BaseDirectory, "Plugins");
    
    private string IconsDirectory => Path.Join(BaseDirectory, "Icons");
    
    private string ConfigDirectory => Path.Join(BaseDirectory, "Config");

    /// <inheritdoc />
    public async Task<StoredPluginData> StorePlugin(Stream fileData) {
        using var archive = new ZipArchive(fileData);

        var archiveEntry = archive.Entries
            .FirstOrDefault(entry => entry.FullName.EndsWith(".uplugin"));
        if (archiveEntry is null) {
            throw new BadSubmissionException("Uplugin file was not found");
        }

        var icon = await archive.Entries
            .FirstOrDefault(x => x.FullName == Path.Join("Resources", "Icon128.png"))
            .ToOption()
            .Match(async x => {
                await using var iconStream = x.Open();
                FileSystem.Directory.CreateDirectory(IconsDirectory);
                var dest = FileSystem.FileInfo.New(Path.Combine(IconsDirectory, $"{Path.GetRandomFileName()}.png"));
                await using var writeStream = dest.Create();
                await iconStream.CopyToAsync(writeStream);
                return (string?) dest.Name;
            }, () => Task.FromResult<string?>(null));

        var fileName =
            $"{Path.GetFileNameWithoutExtension(archiveEntry.FullName)}{Path.GetRandomFileName()}.zip";
        Directory.CreateDirectory(PluginDirectory);
        var fullPath = Path.Combine(PluginDirectory, fileName);
        FileSystem.Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
        await using var fileStream = FileSystem.FileStream.New(fullPath, FileMode.Create);
        fileData.Seek(0, SeekOrigin.Begin);
        await fileData.CopyToAsync(fileStream);

        return new StoredPluginData {
            ZipFile = FileSystem.FileInfo.New(fullPath),
            IconFile = icon
        };
    }

    /// <inheritdoc />
    public Stream RetrievePlugin(IFileInfo fileInfo) {
        return fileInfo.OpenRead();
    }

    /// <inheritdoc />
    public Stream RetrieveIcon(string iconName) {
        return FileSystem.File.OpenRead(Path.Combine(IconsDirectory, iconName));
    }

    /// <inheritdoc />
    public Option<T> GetConfig<T>(string filename) {
        FileSystem.Directory.CreateDirectory(ConfigDirectory);
        var filePath = Path.Combine(ConfigDirectory, filename);
        var fileInfo = FileSystem.FileInfo.New(filePath);
        if (!fileInfo.Exists) {
            return Option<T>.None;   
        }
        using var fileStream = fileInfo.OpenText();
        var configText = fileStream.ReadToEnd();
        var resultObject = JsonSerializer.Deserialize<T>(configText);
        
        return resultObject;
    }

    /// <inheritdoc />
    public T GetConfig<T>(string filename, T defaultValue) {
        var result = GetConfig<T>(filename);
        return result.OrElseGet(() => {
                SaveConfig(filename, defaultValue);
                return defaultValue;
            });
    }

    /// <inheritdoc />
    public T GetConfig<T>(string filename, Func<T> defaultValue) {
        var result = GetConfig<T>(filename);
        return result.OrElseGet(() => {
                var value = defaultValue();
                SaveConfig(filename, value);
                return value;
            });
    }

    /// <inheritdoc />
    public async Task<Option<T>> GetConfigAsync<T>(string filename) {
        FileSystem.Directory.CreateDirectory(ConfigDirectory);
        var filePath = Path.Combine(ConfigDirectory, filename);
        var fileInfo = FileSystem.FileInfo.New(filePath);
        if (!fileInfo.Exists) {
            return Option<T>.None;   
        }
        using var fileStream = fileInfo.OpenText();
        var configText = await fileStream.ReadToEndAsync();
        var resultObject = JsonSerializer.Deserialize<T>(configText);
        
        return resultObject;
    }

    /// <inheritdoc />
    public async Task<T> GetConfigAsync<T>(string filename, T defaultValue) {
        var result = await GetConfigAsync<T>(filename);
        return await result.OrElseGet(async () => {
                await SaveConfigAsync(filename, defaultValue);
                return defaultValue;
            });
    }

    /// <inheritdoc />
    public async Task<T> GetConfigAsync<T>(string filename, Func<T> defaultValue) {
        var result = await GetConfigAsync<T>(filename);
        return await result.OrElseGet(async () => {
                var value = defaultValue();
                await SaveConfigAsync(filename, value);
                return value;
            });
    }

    /// <inheritdoc />
    public void SaveConfig<T>(string filename, T value) {
        ArgumentNullException.ThrowIfNull(value);
        FileSystem.Directory.CreateDirectory(ConfigDirectory);
        var filePath = Path.Combine(ConfigDirectory, filename);
        var fileInfo = FileSystem.FileInfo.New(filePath);
        
        var configText = JsonSerializer.Serialize(value);
        
        using var writer = fileInfo.Open(FileMode.Create, FileAccess.Write, FileShare.None);
        using var textWriter = new StreamWriter(writer);
        textWriter.Write(configText);
    }

    /// <inheritdoc />
    public async Task SaveConfigAsync<T>(string filename, T value) {
        ArgumentNullException.ThrowIfNull(value);
        FileSystem.Directory.CreateDirectory(ConfigDirectory);
        var filePath = Path.Combine(ConfigDirectory, filename);
        var fileInfo = FileSystem.FileInfo.New(filePath);
        
        var configText = JsonSerializer.Serialize(value);
        
        await using var writer = fileInfo.Open(FileMode.Create, FileAccess.Write, FileShare.None);
        await using var textWriter = new StreamWriter(writer);
        await textWriter.WriteAsync(configText);
    }
    
}