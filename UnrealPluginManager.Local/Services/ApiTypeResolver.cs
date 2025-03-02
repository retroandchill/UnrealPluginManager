using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Local.Services;

/// <summary>
/// Resolves and retrieves the interface type of a given API accessor instance.
/// </summary>
public class ApiTypeResolver : IApiTypeResolver {
    /// <inheritdoc />
    public Type GetInterfaceType(IApiAccessor apiAccessor) {
        var concreteType = apiAccessor.GetType();
        var interfaceType = concreteType.GetInterface($"I{concreteType.Name}");
        ArgumentNullException.ThrowIfNull(interfaceType);
        return interfaceType;
    }
}