using System.IO.Abstractions;

namespace UnrealPluginManager.Core.Model.Engine;

public record struct EngineFileData(Version EngineVersion, IFileInfo FileInfo);