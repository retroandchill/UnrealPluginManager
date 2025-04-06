using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Server.Auth;

/// <summary>
/// A filter that enforces an API key authorization mechanism for securing endpoints.
/// </summary>
/// <remarks>
/// This filter is typically used in conjunction with the <see cref="ApiKeyAttribute"/> to
/// validate API key credentials provided in incoming requests. If a valid API key is not
/// detected in the request, access to the endpoint will be denied.
/// </remarks>
/// <example>
/// This filter is registered as a service during application configuration and can be used
/// to guard specific API endpoints against unauthorized access by checking for valid API keys.
/// </example>
[AutoConstructor]
public partial class ApiKeyAuthorizationFilter : IAsyncAuthorizationFilter {

  private const string ApiKeyHeaderName = "X-API-Key";

  private readonly IApiKeyValidator _apiKeyValidator;

  /// <inheritdoc />
  public async Task OnAuthorizationAsync(AuthorizationFilterContext context) {
    // If we have a valid JWT allow us through
    if (context.HttpContext.Request.Headers.Authorization.ToString().StartsWith("Bearer ")) {
      return;
    }

    string? apiKey = context.HttpContext.Request.Headers[ApiKeyHeaderName];
    var foundKeyData = await _apiKeyValidator.LookupApiKey(apiKey);

    foundKeyData.Match(key => {
          var claims = key.PluginGlob.ToOption()
              .Select(g => new Claim(ApiKeyClaims.PluginGlob, g))
              .Concat(key.AllowedPlugins
                  .Select(x => x.Name)
                  .Select(x => new Claim(ApiKeyClaims.AllowedPlugins, x)));
          context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, ApiKeyClaims.AuthenticationType));
        },
        () => context.Result = new UnauthorizedResult());
  }
}