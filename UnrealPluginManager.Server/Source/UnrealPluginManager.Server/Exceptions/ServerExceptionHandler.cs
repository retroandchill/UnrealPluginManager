using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using UnrealPluginManager.Core.Annotations.Exceptions;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Services;

namespace UnrealPluginManager.Server.Exceptions;

/// <summary>
/// Handles exceptions that occur during server operations and provides
/// detailed error responses in a standardized JSON format.
/// </summary>
/// <remarks>
/// This class implements the <see cref="IExceptionHandler"/> interface and is
/// identified by the inclusion of the <see cref="ExceptionHandlerAttribute"/>.
/// It is designed to log error details and return appropriate HTTP responses
/// with a consistent structure. The problem details are serialized to a
/// JSON response with the "application/problem+json" Content-Type header.
/// </remarks>
[AutoConstructor]
[ExceptionHandler]
public partial class ServerExceptionHandler : IExceptionHandler {
  private readonly IHostEnvironment  _env;
  private readonly ILogger<ServerExceptionHandler> _logger;
  private readonly IJsonService _jsonService;
  
  private const string ExceptionFormat = "An exception of type {Type} occurred. Message: {Message}";

  /// <inheritdoc />
  public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken) {
    _logger.LogError(exception, ExceptionFormat, exception.GetType().Name, exception.Message);
    
    var problemDetails = GetProblemDetails(exception, httpContext);
    var json = _jsonService.Serialize(problemDetails);
    
    const string contentType = "application/problem+json";
    httpContext.Response.ContentType = contentType;
    await httpContext.Response.WriteAsync(json, cancellationToken);

    return true;
  }
  
  [GeneralExceptionHandler]
  private partial ProblemDetails GetProblemDetails(Exception exception, HttpContext httpContext);

  [HandlesException]
  private ProblemDetails GetConflictProblemDetails(DependencyConflictException exception, HttpContext httpContext) {
    var details = CreateProblemDetails(httpContext, StatusCodes.Status409Conflict, exception);
    details.Extensions["conflicts"] = exception.Conflicts;
    return details;
  }

  [HandlesException]
  private ProblemDetails GetNotFoundProblemDetails(ContentNotFoundException exception, HttpContext httpContext) {
    return CreateProblemDetails(httpContext, StatusCodes.Status404NotFound, exception);
  }
  
  [HandlesException]
  private ProblemDetails GetBadRequestProblemDetails(BadArgumentException exception, HttpContext httpContext) {
    return CreateProblemDetails(httpContext, StatusCodes.Status400BadRequest, exception);
  }
  

  [FallbackExceptionHandler]
  private ProblemDetails GetDefaultProblemDetails(Exception exception, HttpContext httpContext) {
    return CreateProblemDetails(httpContext, StatusCodes.Status500InternalServerError, exception);
  }

  private ProblemDetails CreateProblemDetails(HttpContext httpContext, int statusCode, Exception exception) {
    var reasonPhrase = ReasonPhrases.GetReasonPhrase(statusCode);
    var details = new ProblemDetails {
      Title = reasonPhrase,
      Status = statusCode,
      Detail = exception.Message,
    };

    if (!_env.IsDevelopment()) {
      return details;
    }
    
    details.Detail = exception.ToString();
    details.Extensions["traceId"] = Activity.Current?.Id;
    details.Extensions["requestId"] = httpContext.TraceIdentifier;

    return details;
  }
}