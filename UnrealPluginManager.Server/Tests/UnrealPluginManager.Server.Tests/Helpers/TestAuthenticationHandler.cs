﻿using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace UnrealPluginManager.Server.Tests.Helpers;

public class TestAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder) {

  protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
    var claims = new[] {
        new Claim(ClaimTypes.Role, "Admin")
    };
    var identity = new ClaimsIdentity(claims, Scheme.Name);
    var principal = new ClaimsPrincipal(identity);
    var ticket = new AuthenticationTicket(principal, Scheme.Name);

    return await Task.FromResult(AuthenticateResult.Success(ticket));
  }
}