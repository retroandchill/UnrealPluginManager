using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UnrealPluginManager.Core.Exceptions;

namespace UnrealPluginManager.Server.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiExceptionFilterAttribute : ExceptionFilterAttribute {
    public override void OnException(ExceptionContext context) {
        context.Result = context.Exception switch {
            DependencyResolutionException or PluginNotFoundException => new NotFoundObjectResult(context.Exception
                .Message),
            BadSubmissionException => new BadRequestObjectResult(context.Exception.Message),
            _ => context.Result
        };
    }
}