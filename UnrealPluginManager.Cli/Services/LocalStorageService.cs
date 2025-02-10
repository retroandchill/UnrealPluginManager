using System.IO.Abstractions;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Cli.Services;

public class LocalStorageService : IStorageService {
    public Task<IFileInfo> StorePlugin(Stream fileData) {
        throw new NotImplementedException();
    }

    public Stream RetrievePlugin(IFileInfo fileInfo) {
        throw new NotImplementedException();
    }
}