using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Cli.Services;

/// <summary>
/// Provides an interface to resolve the type of API accessor interfaces used in the system.
/// </summary>
public interface IApiTypeResolver {
    /// <summary>
    /// Resolves and retrieves the interface type of the provided API accessor instance.
    /// </summary>
    /// <param name="apiAccessor">
    /// The API accessor instance for which the interface type is to be resolved.
    /// </param>
    /// <returns>
    /// The interface type associated with the provided API accessor instance.
    /// </returns>
    Type GetInterfaceType(IApiAccessor apiAccessor);

}