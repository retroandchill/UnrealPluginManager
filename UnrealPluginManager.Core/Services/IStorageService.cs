﻿namespace UnrealPluginManager.Core.Services;

public interface IStorageService {

    Task<FileInfo> StorePlugin(Stream fileData);
    
    Stream RetrievePlugin(FileInfo fileInfo);

}