using System.IO.Abstractions;
using UnrealPluginManager.Core.Model.Plugins.Recipes;

namespace UnrealPluginManager.Local.Services;

public interface ISourceDownloadService {

  Task DownloadAndExtractSources(SourceLocation sourceLocation, IDirectoryInfo directory);

  Task VerifySourceHash(IFileInfo file, string expectedHash);

}