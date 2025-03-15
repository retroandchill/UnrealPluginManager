using System.IO.Abstractions;
using LanguageExt;
using UnrealPluginManager.Core.Files;
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

  private readonly IJsonService _jsonService;

  /// <inheritdoc />
  public abstract string BaseDirectory { get; }

  private string ConfigDirectory => Path.Join(BaseDirectory, "Config");

  /// <inheritdoc />
  public Task<IFileInfo> AddFile(IFileSource fileSource) {
    return fileSource.CreateFile(Path.Join(BaseDirectory, FileSystem.Path.GetRandomFileName()));
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
    var resultObject = _jsonService.Deserialize<T>(configText);

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
    var resultObject = _jsonService.Deserialize<T>(configText);

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

    var configText = _jsonService.Serialize(value);

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

    var configText = _jsonService.Serialize(value);

    await using var writer = fileInfo.Open(FileMode.Create, FileAccess.Write, FileShare.None);
    await using var textWriter = new StreamWriter(writer);
    await textWriter.WriteAsync(configText);
  }
}