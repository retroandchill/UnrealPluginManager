using System.IO.Abstractions;
using LanguageExt;
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
  /// Represents the directory path used for storing resource files specific to the storage service.
  /// This property combines the base directory and a predefined folder name to provide
  /// a consistent location for resource management within the application.
  /// </summary>
  string ResourceDirectory { get; }

  /// <summary>
  /// Adds a file to the storage system based on the provided file source.
  /// </summary>
  /// <param name="fileSource">The source of the file to be added. It defines the origin and content of the file.</param>
  /// <returns>A task that represents the asynchronous operation, containing the file information of the added file.</returns>
  Task<ResourceHandle> AddResource(IFileSource fileSource);

  /// <summary>
  /// Retrieves information about a resource file from the storage system using the provided filename.
  /// </summary>
  /// <param name="filename">The name of the resource file to retrieve information for.</param>
  /// <returns>An <see cref="IFileInfo"/> instance representing the file information of the specified resource file.</returns>
  ResourceHandle RetrieveResourceInfo(string filename);

  /// <summary>
  /// Retrieves a read-only file stream for the specified resource file.
  /// </summary>
  /// <param name="filename">The name of the resource file to retrieve the stream for.</param>
  /// <returns>A stream that provides read-only access to the contents of the resource file.</returns>
  Stream GetResourceStream(string filename);

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