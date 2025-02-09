using System.IO.Abstractions;

namespace UnrealPluginManager.Core.Services;

public interface IStorageService {

    Task<IFileInfo> StorePlugin(Stream fileData);
    
    Stream RetrievePlugin(IFileInfo fileInfo);

}