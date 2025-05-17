using Microsoft.Extensions.DependencyInjection;
using UnrealPluginManager.Server.DependencyInjection;
using UnrealPluginManager.Server.Utils;

namespace UnrealPluginManager.Server.Tests.Helpers;

public class TestServerServiceProviderFactory : IServiceProviderFactory<IServiceCollection> {
  public IServiceCollection CreateBuilder(IServiceCollection services) {
    return services;
  }

  public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder) {
    return containerBuilder
        .AddJabServices(p => new TestServerServiceProvider(p))
        .ConfigureAuth()
        .BuildServiceProvider();
  }
}