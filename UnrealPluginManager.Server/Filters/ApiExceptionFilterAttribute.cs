using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using UnrealPluginManager.Core.Exceptions;

namespace UnrealPluginManager.Server.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiExceptionFilterAttribute : ExceptionFilterAttribute {
    public override void OnException(ExceptionContext context) {
        if (context.Exception is DependencyResolutionException) {
            context.Result = new NotFoundObjectResult(context.Exception.Message);
        }
    }
}