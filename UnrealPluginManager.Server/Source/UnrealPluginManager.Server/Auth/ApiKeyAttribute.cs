using Microsoft.AspNetCore.Mvc;

namespace UnrealPluginManager.Server.Auth;

/// <summary>
/// Represents an attribute that is used to enforce API key-based authorization
/// for access control in an ASP.NET Core application.
/// </summary>
/// <remarks>
/// This attribute applies a filter that validates the presence and correctness of an API key
/// in the incoming request. By applying this attribute to a controller or an action,
/// you can restrict access to those endpoints based on the API key validation logic.
/// The validation logic is implemented in the <c>ApiKeyAuthorizationFilter</c>.
/// This filter ensures that requests without a valid API key do not proceed and are
/// rejected with the appropriate response.
/// </remarks>
public class ApiKeyAttribute() : ServiceFilterAttribute(typeof(ApiKeyAuthorizationFilter));