using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
    string? apiKey = context.HttpContext.Request.Headers[ApiKeyHeaderName];
    if (!await _apiKeyValidator.IsValid(apiKey)) {
      context.Result = new UnauthorizedResult();
    }
  }
}