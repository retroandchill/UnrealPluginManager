using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Cli.Factories;

/// <summary>
/// Interface for a factory that provides instances of API accessors.
/// </summary>
/// <typeparam name="T">The type of API accessor that implements the <see cref="IApiAccessor"/> interface.</typeparam>
public interface IApiAccessorFactory<T> where T : IApiAccessor {

    /// <summary>
    /// Retrieves the collection of API accessors managed by the factory.
    /// </summary>
    /// <returns>
    /// A task representing the asynchronous operation, which returns an ordered dictionary
    /// where the keys are strings, and the values are instances of the API accessor type.
    /// </returns>
    Task<OrderedDictionary<string, T>> GetAccessors();
    
}