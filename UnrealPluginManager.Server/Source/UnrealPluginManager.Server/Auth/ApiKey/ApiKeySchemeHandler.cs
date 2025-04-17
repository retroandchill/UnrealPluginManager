using System.Reflection;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Server.Auth.ApiKey;

/// <summary>
/// Represents the options for configuring the API key authentication scheme.
/// </summary>
/// <remarks>
/// This class is used to configure settings for authentication using API keys.
/// It inherits from <see cref="AuthenticationSchemeOptions"/> and provides additional
/// customization options specific to API key authentication.
/// </remarks>
public class ApiKeySchemeOptions : AuthenticationSchemeOptions;

/// <summary>
/// Handles API key authentication for the application.
/// </summary>
/// <remarks>
/// This class provides the implementation for authenticating requests using the API key
/// authentication scheme. It derives from <see cref="AuthenticationHandler{ApiKeySchemeOptions}"/> and overrides
/// the <c>HandleAuthenticateAsync</c> method to validate API keys and generate corresponding claims.
/// </remarks>
public class ApiKeySchemeHandler(IOptionsMonitor<ApiKeySchemeOptions> options,
                                 ILoggerFactory logger,
                                 UrlEncoder encoder,
                                 IApiKeyValidator apiKeyValidator)
    : AuthenticationHandler<ApiKeySchemeOptions>(options, logger, encoder) {

  private const string ApiKeyHeaderName = "X-API-Key";
  
  private readonly IApiKeyValidator _apiKeyValidator =  apiKeyValidator;
  
  /// <inheritdoc />
  protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
    // If we have a valid JWT allow us through
    if (Request.Headers.Authorization.ToString().StartsWith("Bearer ")) {
      return AuthenticateResult.NoResult();
    }
    
    var endpoint = Context.GetEndpoint();
    var descriptor = endpoint?.Metadata.GetMetadata<ControllerActionDescriptor>();
    if (descriptor is not null) {
      var apiKeyAttribute = descriptor.MethodInfo.GetCustomAttribute<ApiKeyAttribute>() ?? 
                            descriptor.ControllerTypeInfo.GetCustomAttribute<ApiKeyAttribute>();
      if (apiKeyAttribute is null) {
        return AuthenticateResult.Fail("Can't use API key authentication for this endpoint.");
      }
    }

    string? apiKey = Request.Headers[ApiKeyHeaderName];
    var foundKeyData = await _apiKeyValidator.LookupApiKey(apiKey);

    return foundKeyData.Match(key => {
                         var claims = key.PluginGlob.ToOption()
                             .Select(g => new Claim(ApiKeyClaims.PluginGlob, g))
                             .Concat(key.AllowedPlugins
                                         .Select(x => x.Name)
                                         .Select(x => new Claim(ApiKeyClaims.AllowedPlugins, x)));
                         var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, ApiKeyClaims.AuthenticationType));
                         var claimsTicket = new AuthenticationTicket(principal, Scheme.Name);
                         return AuthenticateResult.Success(claimsTicket);
                       },
                       () => AuthenticateResult.Fail("Invalid or missing API key"));
  }
}