using System.IO.Abstractions;

namespace UnrealPluginManager.Core.Model.Storage;

/// <summary>
/// Defines the structure of data for a stored plugin, including its zip file and optional icon information.
/// </summary>
public record struct StoredPluginData {
    /// <summary>
    /// Gets the file information of the plugin's associated zip file.
    /// </summary>
    /// <remarks>
    /// The property represents the zip archive containing the plugin's data,
    /// allowing access to metadata or operations provided by the file abstraction.
    /// </remarks>
    public required IFileInfo ZipFile { get; init; }

    /// <summary>
    /// Gets the file information of the plugin's associated icon file.
    /// </summary>
    /// <remarks>
    /// The property represents an optional file that contains the visual representation or icon of the plugin,
    /// providing additional contextual details about the plugin's appearance.
    /// </remarks>
    public string? IconFile { get; init; }
};