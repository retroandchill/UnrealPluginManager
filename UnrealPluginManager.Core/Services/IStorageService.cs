using System.IO.Abstractions;
using LanguageExt;
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

    /// <summary>
    /// Retrieves the configuration data of the specified type from the given filename, if the file exists.
    /// </summary>
    /// <param name="filename">The name of the configuration file to retrieve.</param>
    /// <typeparam name="T">The type to which the configuration data should be deserialized.</typeparam>
    /// <returns>An <see cref="Option{T}"/> containing the configuration data if the file exists, or an empty option if it does not.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the file extension is not supported for deserialization.</exception>
    Task<Option<T>> GetConfig<T>(string filename);

    /// <summary>
    /// Retrieves the configuration data from a specified file, or uses the provided default value if the file is unavailable or the configuration is not found.
    /// </summary>
    /// <param name="filename">The name of the configuration file to be retrieved.</param>
    /// <param name="defaultValue">The default value to be used if the configuration file is unavailable or the configuration is not found.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the configuration data as an instance of <typeparamref name="T"/>.</returns>
    Task<T> GetConfig<T>(string filename, T defaultValue);

    /// <summary>
    /// Retrieves the configuration data from the specified file or uses the provided factory method to generate a default value if the file is not found or contains no valid data.
    /// </summary>
    /// <param name="filename">The name of the file containing the configuration to be retrieved.</param>
    /// <param name="defaultValue">A function that provides a default value to be used if the configuration file is not found or empty.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the configuration data of type <typeparamref name="T"/>.</returns>
    Task<T> GetConfig<T>(string filename, Func<T> defaultValue);

    /// <summary>
    /// Saves the specified configuration data to a file.
    /// </summary>
    /// <param name="filename">The name of the configuration file in which the data will be saved.</param>
    /// <param name="value">The configuration data to be saved.</param>
    /// <typeparam name="T">The type of the configuration data to save.</typeparam>
    /// <returns>A <see cref="Task"/> that represents the asynchronous save operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the file extension is not a supported configuration type (e.g., JSON or YAML).</exception>
    Task SaveConfig<T>(string filename, T value);
}