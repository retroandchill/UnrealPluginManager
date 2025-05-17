using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UnrealPluginManager.Core.Utils;
using UnrealPluginManager.Server.DependencyInjection;
using UnrealPluginManager.Server.Utils;

namespace UnrealPluginManager.Server.Tests.Helpers;

public class TestServerServiceProviderFactory : IServiceProviderFactory<IServiceCollection> {
  public IServiceCollection CreateBuilder(IServiceCollection services) {
    return services;
  }

  public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder) {
    var configuration = containerBuilder.Where(x => x.ServiceType == typeof(IConfiguration))
        .Select(x => x.ImplementationInstance ?? x.ImplementationFactory?.Invoke(null))
        .OfType<IConfiguration>()
        .FirstOrDefault();

    var services = containerBuilder
        .AddJabServices(p => new TestServerServiceProvider(p))
        .ConfigureAuth(configuration.RequireNonNull());

    services.AddAuthentication("Test")
        .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", null);

    return services.BuildServiceProvider();
  }
}