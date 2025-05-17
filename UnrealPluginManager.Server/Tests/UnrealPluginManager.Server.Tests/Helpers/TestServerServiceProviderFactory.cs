using Microsoft.Extensions.DependencyInjection;
using UnrealPluginManager.Server.DependencyInjection;

namespace UnrealPluginManager.Server.Tests.Helpers;

public class TestServerServiceProviderFactory : IServiceProviderFactory<IServiceCollection> {
  public IServiceCollection CreateBuilder(IServiceCollection services) {
    return services;
  }

  public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder) {
    return containerBuilder
        .AddJabServices(p => new TestServerServiceProvider(p))
        .BuildServiceProvider();
  }
}