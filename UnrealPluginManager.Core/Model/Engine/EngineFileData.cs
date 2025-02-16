using System.IO.Abstractions;

namespace UnrealPluginManager.Core.Model.Engine;

/// <summary>
/// Represents data associated with an engine file, which includes the version of the engine and the file information.
/// </summary>
/// <param name="EngineVersion">
/// The version of the engine associated with the file.
/// </param>
/// <param name="FileInfo">
/// Provides details about the file, such as its path and metadata, using an abstraction of the file system.
/// </param>
public record struct EngineFileData(Version EngineVersion, IFileInfo FileInfo, string Platform = "Win64");