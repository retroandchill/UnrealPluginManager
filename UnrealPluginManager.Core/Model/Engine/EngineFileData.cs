using System.IO.Abstractions;
using UnrealPluginManager.Core.Model.Storage;

namespace UnrealPluginManager.Core.Model.Engine;

/// <summary>
/// Represents data associated with a specific engine file, incorporating the engine version,
/// file information, and optional platform details.
/// </summary>
/// <param name="EngineVersion">
/// Specifies the version of the engine tied to the file.
/// </param>
/// <param name="FileInfo">
/// Contains metadata and path information about the file, utilizing a file system abstraction.
/// </param>
/// <param name="Platform">
/// Indicates the platform for which the engine file is designated, with a default value of "Win64".
/// </param>
public record struct EngineFileData(Version EngineVersion, StoredPluginData FileInfo, string Platform = "Win64");