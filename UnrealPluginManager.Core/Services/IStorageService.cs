using System.IO.Abstractions;
using UnrealPluginManager.Core.Exceptions;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Represents a service interface for managing the storage of plugins.
/// Provides methods to store and retrieve plugin data.
/// </summary>
public interface IStorageService {
    /// <summary>
    /// Stores the provided plugin data stream.
    /// </summary>
    /// <param name="fileData">The stream containing the plugin data to be stored.</param>
    /// <returns>An <see cref="IFileInfo"/> object representing the stored plugin file.</returns>
    /// <exception cref="BadSubmissionException">Thrown when a .uplugin file is not found in the provided stream.</exception>
    Task<IFileInfo> StorePlugin(Stream fileData);

    /// <summary>
    /// Retrieves the plugin data stream for the specified plugin file.
    /// </summary>
    /// <param name="fileInfo">The file information object representing the plugin file to be retrieved.</param>
    /// <returns>A <see cref="Stream"/> representing the contents of the retrieved plugin file.</returns>
    Stream RetrievePlugin(IFileInfo fileInfo);
}