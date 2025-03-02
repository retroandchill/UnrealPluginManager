using UnrealPluginManager.Local.Services;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Local.Tests.Mocks;

public class MockTypeResolver : IApiTypeResolver {
    public Type GetInterfaceType(IApiAccessor apiAccessor) {
        // The API type should be the first one
        return apiAccessor.GetType().GetInterfaces().First();
    }
}