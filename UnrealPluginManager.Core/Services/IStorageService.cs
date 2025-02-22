using System.IO.Abstractions;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Model.Storage;

namespace UnrealPluginManager.Core.Services;

/// <summary>
/// Represents a service interface for managing the storage of plugins.
/// Provides methods to store and retrieve plugin data.
/// </summary>
public interface IStorageService {
    /// <summary>
    /// Gets the base directory used by the storage service for saving and retrieving plugin data.
    /// This directory is typically the foundational location where plugin-related data files
    /// and configurations are stored.
    /// </summary>
    string BaseDirectory { get; }

    /// <summary>
    /// Stores the provided plugin data stream.
    /// </summary>
    /// <param name="fileData">The stream containing the plugin data to be stored.</param>
    /// <returns>An <see cref="IFileInfo"/> object representing the stored plugin file.</returns>
    /// <exception cref="BadSubmissionException">Thrown when a .uplugin file is not found in the provided stream.</exception>
    Task<StoredPluginData> StorePlugin(Stream fileData);

    /// <summary>
    /// Retrieves the plugin data stream for the specified plugin file.
    /// </summary>
    /// <param name="fileInfo">The file information object representing the plugin file to be retrieved.</param>
    /// <returns>A <see cref="Stream"/> representing the contents of the retrieved plugin file.</returns>
    Stream RetrievePlugin(IFileInfo fileInfo);

    /// <summary>
    /// Retrieves the icon data stream for the specified icon name.
    /// </summary>
    /// <param name="iconName">The name of the icon to be retrieved.</param>
    /// <returns>A <see cref="Stream"/> representing the contents of the retrieved icon.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified icon file is not found.</exception>
    Stream RetrieveIcon(string iconName);
}