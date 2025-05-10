using System.IO.Abstractions;

namespace UnrealPluginManager.Local.Model.Engine;

public record BuildResult(int ExitCode, IDirectoryInfo Directory);