using System.Reflection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using UnrealPluginManager.Server.Auth.ApiKey;

namespace UnrealPluginManager.ApiGenerator.Swagger;

/// <summary>
/// Represents an operation filter that applies security requirements to API operations.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IOperationFilter"/> interface, enabling customization of Swagger generation
/// by modifying or enhancing the operation metadata with security-related configurations.
/// </remarks>
/// <seealso cref="IOperationFilter" />
public class SecurityRequirementsOperationFilter : IOperationFilter {

  /// <inheritdoc />
  public void Apply(OpenApiOperation operation, OperationFilterContext context) {
    var requiredScopes = context.MethodInfo
        .GetCustomAttributes(true)
        .OfType<AuthorizeAttribute>()
        .Select(attr => attr.Policy!)
        .Where(scope => !string.IsNullOrWhiteSpace(scope))
        .Distinct()
        .ToList();

    operation.Responses.Add("401", new OpenApiResponse {
        Description = "Unauthorized"
    });
    operation.Responses.Add("403", new OpenApiResponse {
        Description = "Forbidden"
    });

    var oAuthScheme = new OpenApiSecurityScheme {
        Reference = new OpenApiReference {
            Type = ReferenceType.SecurityScheme,
            Id = "oauth2"
        }
    };

    operation.Security = new List<OpenApiSecurityRequirement> {
        new() {
            [oAuthScheme] = requiredScopes
        }
    };

    var apiKeyAttribute = context.MethodInfo.GetCustomAttribute<ApiKeyAttribute>();
    if (apiKeyAttribute is null) {
      return;
    }
    var apiKeyScheme = new OpenApiSecurityScheme {
        Reference = new OpenApiReference {
            Type = ReferenceType.SecurityScheme,
            Id = "apiKey"
        }
    };

    operation.Security.Add(new OpenApiSecurityRequirement {
        [apiKeyScheme] = requiredScopes
    });
  }
}