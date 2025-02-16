using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using UnrealPluginManager.Server.Controllers;

namespace UnrealPluginManager.ApiGenerator.Utils;

public static class EndpointUtils {

    public static WebApplicationBuilder ConfigureEndpoints(this WebApplicationBuilder builder) {
        builder.Services.AddMvc()
            .AddApplicationPart(typeof(PluginsController).Assembly)
            .AddControllersAsServices();
        return builder;
    }

}