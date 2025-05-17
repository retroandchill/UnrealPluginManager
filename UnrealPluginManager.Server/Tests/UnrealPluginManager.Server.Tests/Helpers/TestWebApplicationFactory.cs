using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace UnrealPluginManager.Server.Tests.Helpers;

public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class {


  protected override void ConfigureWebHost(IWebHostBuilder builder) {
    builder.ConfigureTestServices(services => {
      services.AddAuthentication("Test")
          .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", null);
    });

    builder.UseEnvironment("Development");
  }

  protected override IHost CreateHost(IHostBuilder builder) {
    builder.UseServiceProviderFactory(new TestServerServiceProviderFactory());
    var host = builder.Build();
    host.Start();
    return host;
  }

}