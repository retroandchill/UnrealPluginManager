using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UnrealPluginManager.Core.Exceptions;

namespace UnrealPluginManager.Server.Filters;

/// <summary>
/// Represents a custom exception filter attribute for handling exceptions in ASP.NET Core applications.
/// </summary>
/// <remarks>
/// This filter processes exceptions that occur within the application, providing uniform handling for specific
/// exception types by returning appropriate HTTP responses. Exceptions like DependencyResolutionException,
/// PluginNotFoundException, and BadSubmissionException are handled in a standardized manner.
/// </remarks>
/// <example>
/// Apply this attribute to controller actions or controllers to enable exception handling for API endpoints.
/// The filter transforms specific exceptions into HTTP responses, such as 404 Not Found or 400 Bad Request,
/// based on the exception type.
/// </example>
/// <seealso cref="Microsoft.AspNetCore.Mvc.Filters.ExceptionFilterAttribute"/>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiExceptionFilterAttribute : ExceptionFilterAttribute {
  /// <inheritdoc />
  public override void OnException(ExceptionContext context) {
    context.Result = context.Exception switch {
        PluginNotFoundException => new NotFoundObjectResult(context.Exception
              .Message),
        BadSubmissionException => new BadRequestObjectResult(context.Exception.Message),
        _ => context.Result
    };
  }
}