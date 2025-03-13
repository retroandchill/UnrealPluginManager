namespace UnrealPluginManager.Core.Model.Plugins;

/// <summary>
/// Represents the data required for downloading a plugin, including the plugin's name and its associated file data.
/// </summary>
/// <remarks>
/// This struct is commonly used as a return type when retrieving a plugin file.
/// It encapsulates the plugin name and the file data in the form of a stream, which can be used for further processing or storage.
/// </remarks>
/// <param name="PluginName">
/// The name of the plugin to be downloaded.
/// </param>
/// <param name="FileData">
/// The file data of the plugin in the form of a stream.
/// This could represent the content of the plugin file, allowing for reading or saving the file.
/// </param>
public readonly record struct PluginDownload(string PluginName, Stream FileData) : IDisposable, IAsyncDisposable {

  /// <inheritdoc />
  public void Dispose() {
    FileData.Dispose();
  }
  
  /// <inheritdoc />
  public async ValueTask DisposeAsync() {
    await FileData.DisposeAsync();
  }
}