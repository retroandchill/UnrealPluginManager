using System.Diagnostics;
using AutoExceptionHandler.Annotations;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Retro.ReadOnlyParams.Annotations;
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
[ExceptionHandler]
public partial class ServerExceptionHandler(
    [ReadOnly] IHostEnvironment env,
    [ReadOnly] ILogger<ServerExceptionHandler> logger,
    [ReadOnly] IJsonService jsonService) : IExceptionHandler {
  private const string ExceptionFormat = "An exception of type {Type} occurred. Message: {Message}";

  /// <inheritdoc />
  public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
                                              CancellationToken cancellationToken) {
    logger.LogError(exception, ExceptionFormat, exception.GetType().Name, exception.Message);

    var problemDetails = GetProblemDetails(exception, httpContext);
    var json = jsonService.Serialize(problemDetails);

    const string contentType = "application/problem+json";
    httpContext.Response.ContentType = contentType;
    httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
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

  [HandlesException]
  private ProblemDetails GetForeignApiPropProblemDetails(ForeignApiException exception, HttpContext httpContext) {
    return CreateProblemDetails(httpContext, exception.StatusCode, exception);
  }


  [FallbackExceptionHandler]
  private ProblemDetails GetDefaultProblemDetails(Exception exception, HttpContext httpContext) {
    return CreateProblemDetails(httpContext, StatusCodes.Status500InternalServerError,
                                "An unexpected server error occurred", exception);
  }

  private ProblemDetails CreateProblemDetails(HttpContext httpContext, int statusCode, Exception exception) {
    return CreateProblemDetails(httpContext, statusCode, exception.Message, exception);
  }

  private ProblemDetails CreateProblemDetails(HttpContext httpContext, int statusCode, string message,
                                              Exception exception) {
    var reasonPhrase = ReasonPhrases.GetReasonPhrase(statusCode);
    var details = new ProblemDetails {
        Title = reasonPhrase,
        Status = statusCode,
        Detail = message,
    };

    if (!env.IsDevelopment()) {
      return details;
    }

    details.Detail = exception.ToString();
    details.Extensions["traceId"] = Activity.Current?.Id;
    details.Extensions["requestId"] = httpContext.TraceIdentifier;

    return details;
  }
}