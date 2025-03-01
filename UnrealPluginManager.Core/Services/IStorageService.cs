using System.IO.Abstractions;
using LanguageExt;
using Semver;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Files;
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
    /// Stores the source files of a plugin using the provided file source.
    /// </summary>
    /// <param name="pluginName">The name of the plugin for which the source files are being stored.</param>
    /// <param name="version">The version of the plugin.</param>
    /// <param name="fileSource">The file source interface used to create the plugin source files.</param>
    /// <returns>An <see cref="IFileInfo"/> object representing the stored plugin source file.</returns>
    Task<IFileInfo> StorePluginSource(string pluginName, SemVersion version, IFileSource fileSource);

    /// <summary>
    /// Retrieves the source file of a plugin based on the specified plugin name and version.
    /// </summary>
    /// <param name="pluginName">The name of the plugin whose source file is being retrieved.</param>
    /// <param name="version">The version of the plugin.</param>
    /// <returns>A <see cref="Stream"/> containing the plugin source file.</returns>
    Stream RetrievePluginSource(string pluginName, SemVersion version);

    /// <summary>
    /// Stores the icon file of a plugin using the provided file source.
    /// </summary>
    /// <param name="pluginName">The name of the plugin for which the icon file is being stored.</param>
    /// <param name="iconFile">The file source interface used to create the plugin icon file.</param>
    /// <returns>An <see cref="IFileInfo"/> object representing the stored plugin icon file.</returns>
    Task<IFileInfo> StorePluginIcon(string pluginName, IFileSource iconFile);

    /// <summary>
    /// Retrieves the icon file stream associated with the specified plugin.
    /// </summary>
    /// <param name="pluginName">The name of the plugin for which the icon is being retrieved.</param>
    /// <returns>A <see cref="Stream"/> representing the plugin's icon file.</returns>
    Stream RetrievePluginIcon(string pluginName);

    /// <summary>
    /// Stores the binary files of a plugin for a specified version, engine version, and platform using the provided file source.
    /// </summary>
    /// <param name="pluginName">The name of the plugin for which the binary files are being stored.</param>
    /// <param name="version">The version of the plugin.</param>
    /// <param name="engineVersion">The version of the engine that the plugin binaries are compatible with.</param>
    /// <param name="platform">The target platform for the plugin binaries.</param>
    /// <param name="binariesFile">The file source interface used to create the plugin binary files.</param>
    /// <returns>An <see cref="IFileInfo"/> object representing the stored plugin binary file.</returns>
    Task<IFileInfo> StorePluginBinaries(string pluginName, SemVersion version, string engineVersion, string platform, 
                                        IFileSource binariesFile);

    /// <summary>
    /// Retrieves the binary files of a plugin for the specified version, engine version, and platform.
    /// </summary>
    /// <param name="pluginName">The name of the plugin whose binaries are being retrieved.</param>
    /// <param name="version">The version of the plugin.</param>
    /// <param name="engineVersion">The version of the engine for which the binaries are being retrieved.</param>
    /// <param name="platform">The platform for which the binaries are being retrieved.</param>
    /// <returns>A <see cref="Stream"/> containing the plugin binary files.</returns>
    Stream RetrievePluginBinaries(string pluginName, SemVersion version, string engineVersion, string platform);

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
    /// Retrieves the configuration object of a specified type from the given configuration file.
    /// </summary>
    /// <param name="filename">The name of the configuration file to read from.</param>
    /// <typeparam name="T">The type of the configuration object to retrieve.</typeparam>
    /// <returns>An <see cref="Option{T}"/> containing the deserialized configuration object if found, or none if the file does not exist or deserialization fails.</returns>
    Option<T> GetConfig<T>(string filename);

    /// <summary>
    /// Retrieves the specified configuration from a file.
    /// </summary>
    /// <param name="filename">The name of the file containing the configuration.</param>
    /// <param name="defaultValue">The default value to return if the file is not found or cannot be loaded.</param>
    /// <typeparam name="T">The type of the configuration to retrieve.</typeparam>
    /// <returns>A configuration object of type <typeparamref name="T"/> if found; otherwise, the provided default value of type <typeparamref name="T"/>.</returns>
    T GetConfig<T>(string filename, T defaultValue);

    /// <summary>
    /// Retrieves the configuration data of the specified type from the given file, or returns the default value provided by the specified function if the configuration is not found.
    /// </summary>
    /// <param name="filename">The name of the file containing the configuration data.</param>
    /// <param name="defaultValue">A function that provides the default value to return if the configuration data is not found.</param>
    /// <typeparam name="T">The type of the configuration data to retrieve.</typeparam>
    /// <returns>The configuration data of type <typeparamref name="T"/> retrieved from the file, or the default value provided by the function.</returns>
    T GetConfig<T>(string filename, Func<T> defaultValue);

    /// <summary>
    /// Retrieves the configuration data of the specified type from the given filename, if the file exists.
    /// </summary>
    /// <param name="filename">The name of the configuration file to retrieve.</param>
    /// <typeparam name="T">The type to which the configuration data should be deserialized.</typeparam>
    /// <returns>An <see cref="Option{T}"/> containing the configuration data if the file exists, or an empty option if it does not.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the file extension is not supported for deserialization.</exception>
    Task<Option<T>> GetConfigAsync<T>(string filename);

    /// <summary>
    /// Retrieves the configuration data from a specified file, or uses the provided default value if the file is unavailable or the configuration is not found.
    /// </summary>
    /// <param name="filename">The name of the configuration file to be retrieved.</param>
    /// <param name="defaultValue">The default value to be used if the configuration file is unavailable or the configuration is not found.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the configuration data as an instance of <typeparamref name="T"/>.</returns>
    Task<T> GetConfigAsync<T>(string filename, T defaultValue);

    /// <summary>
    /// Retrieves the configuration data from the specified file or uses the provided factory method to generate a default value if the file is not found or contains no valid data.
    /// </summary>
    /// <param name="filename">The name of the file containing the configuration to be retrieved.</param>
    /// <param name="defaultValue">A function that provides a default value to be used if the configuration file is not found or empty.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the configuration data of type <typeparamref name="T"/>.</returns>
    Task<T> GetConfigAsync<T>(string filename, Func<T> defaultValue);

    /// <summary>
    /// Saves the specified configuration value to a file with the given filename.
    /// </summary>
    /// <param name="filename">The name of the file where the configuration data will be saved.</param>
    /// <param name="value">The configuration value to be saved in the specified file.</param>
    /// <typeparam name="T">The type of the configuration value.</typeparam>
    /// <exception cref="ArgumentNullException">Thrown when the provided configuration value is null.</exception>
    void SaveConfig<T>(string filename, T value);

    /// <summary>
    /// Saves the specified configuration data to a file.
    /// </summary>
    /// <param name="filename">The name of the configuration file in which the data will be saved.</param>
    /// <param name="value">The configuration data to be saved.</param>
    /// <typeparam name="T">The type of the configuration data to save.</typeparam>
    /// <returns>A <see cref="Task"/> that represents the asynchronous save operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the provided value is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the file extension is not a supported configuration type (e.g., JSON or YAML).</exception>
    Task SaveConfigAsync<T>(string filename, T value);
}