using System.IO.Abstractions;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Core.Tests.Mocks;

public class MockStorageService : StorageServiceBase {

  public MockStorageService(IFileSystem filesystem, IJsonService jsonService) : base(filesystem, jsonService) {
    BaseDirectory = filesystem.Path.Combine(filesystem.Directory.GetCurrentDirectory(), "UnrealPluginManager");
    ResourceDirectory = filesystem.Path.Combine(BaseDirectory, "Resources");

    filesystem.Directory.CreateDirectory(BaseDirectory);
    filesystem.Directory.CreateDirectory(ResourceDirectory);
  }

  public sealed override string BaseDirectory { get; }
  public sealed override string ResourceDirectory { get; }
}