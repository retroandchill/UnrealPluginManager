using System.CommandLine.Builder;
using System.CommandLine.Invocation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace UnrealPluginManager.Cli.DependencyInjection;

/// <summary>
/// Provides middleware for integrating dependency injection with the command line processing pipeline.
/// </summary>
public static class DependencyInjectionMiddleware {
    /// <summary>
    /// Adds dependency injection to the command line processing pipeline
    /// by configuring the service collection with the provided configuration action.
    /// </summary>
    /// <param name="builder">The <see cref="CommandLineBuilder"/> instance to apply the dependency injection middleware to.</param>
    /// <param name="configureServices">An action to configure the services using the provided <see cref="ServiceCollection"/>.</param>
    /// <returns>The modified <see cref="CommandLineBuilder"/> instance, allowing further configuration.</returns>
    public static CommandLineBuilder UseDependencyInjection(this CommandLineBuilder builder, Action<ServiceCollection> configureServices) {
        return builder.UseDependencyInjection((_, services) => configureServices(services));
    }

    /// <summary>
    /// Adds dependency injection to the command line processing pipeline by configuring the service collection with the provided configuration action.
    /// </summary>
    /// <param name="builder">The <see cref="CommandLineBuilder"/> instance to apply the dependency injection middleware to.</param>
    /// <param name="configureServices">An action to configure the services using the provided <see cref="InvocationContext"/> and <see cref="ServiceCollection"/>.</param>
    /// <returns>The modified <see cref="CommandLineBuilder"/> instance, allowing further configuration.</returns>
    private static CommandLineBuilder UseDependencyInjection(this CommandLineBuilder builder, Action<InvocationContext, ServiceCollection> configureServices) {
        return builder.AddMiddleware(async (context, next) => {
            var services = new ServiceCollection();
            configureServices(context, services);
            var uniqueServiceTypes = services.Select(x => x.ServiceType)
                .ToHashSet();

            services.TryAddSingleton(context.Console);

            await using var serviceProvider = services.BuildServiceProvider();

            context.BindingContext.AddService<IServiceProvider>(_ => serviceProvider);

            foreach (var serviceType in uniqueServiceTypes) {
                context.BindingContext.AddService(serviceType, _ => serviceProvider.GetRequiredService(serviceType));

                var enumerableServiceType = typeof(IEnumerable<>).MakeGenericType(serviceType);
                context.BindingContext.AddService(enumerableServiceType, _ => serviceProvider.GetServices(serviceType));
            }

            await next(context);
        });
    }
    
}