using UnrealPluginManager.Cli.Services;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.WebClient.Client;

namespace UnrealPluginManager.Cli.Factories;

/// <summary>
/// Factory class for creating and managing instances of API accessors.
/// </summary>
/// <typeparam name="T">The type of API accessor that implements the <see cref="IApiAccessor"/> interface.</typeparam>
/// <typeparam name="TImplementation">
/// The concrete implementation of the API accessor type, which must also implement <typeparamref name="T"/>.
/// </typeparam>
public class ApiAccessorFactory<T, TImplementation> : IApiAccessorFactory<T> where T : IApiAccessor where TImplementation : T {
    private readonly IRemoteService _remoteService;
    private readonly Lazy<Task<OrderedDictionary<string, T>>> _accessors;

    /// <summary>
    /// Factory class for creating and managing instances of API accessors.
    /// </summary>
    /// <typeparam name="T">The type of API accessor that implements the <see cref="IApiAccessor"/> interface.</typeparam>
    /// <typeparam name="TImplementation">
    /// The concrete implementation of the API accessor type, which must also implement <typeparamref name="T"/>.
    /// </typeparam>
    public ApiAccessorFactory(IRemoteService remoteService) {
        _remoteService = remoteService;
        _accessors = new Lazy<Task<OrderedDictionary<string, T>>>(CreateAccessors);
    }

    private async Task<OrderedDictionary<string, T>> CreateAccessors() {
        var remotes = await _remoteService.GetAllRemotes();
        var constructor = typeof(TImplementation).GetConstructor([typeof(string)]);
        ArgumentNullException.ThrowIfNull(constructor);
        return remotes.ToOrderedDictionary(x => {
            var newAccessor = (T) constructor.Invoke([x.Url.ToString()]);
            ArgumentNullException.ThrowIfNull(newAccessor);
            return newAccessor;
        });
    }

    /// <inheritdoc />
    public Task<OrderedDictionary<string, T>> GetAccessors() {
        return _accessors.Value;
    }
}