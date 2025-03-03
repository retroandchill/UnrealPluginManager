﻿using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using UnrealPluginManager.Core.Pagination;

namespace UnrealPluginManager.ApiGenerator.Swagger;

/// <summary>
/// The PageableParameterFilter class is an implementation of the IOperationFilter interface for modifying the
/// OpenAPI documentation generated by Swagger. It customizes the API input parameters to standardize
/// pagination handling by adding "page" and "size" query parameters where the endpoint accepts a <see cref="Pageable"/> object.
/// </summary>
/// <remarks>
/// This filter ensures that methods accepting a <see cref="Pageable"/> parameter are represented with the
/// appropriate "page" and "size" query parameters in the OpenAPI specification, improving clarity and consistency
/// for API consumers.
/// </remarks>
/// <example>
/// This filter would replace parameters related to properties from the <see cref="Pageable"/> class
/// with "page" and "size" query parameters, each having constraints such as default values and minimum/maximum range.
/// </example>
/// <seealso cref="Swashbuckle.AspNetCore.SwaggerGen.IOperationFilter" />
public class PageableParameterFilter : IOperationFilter {
  /// <inheritdoc />
  public void Apply(OpenApiOperation operation, OperationFilterContext context) {
    var pageableValue = context.MethodInfo.GetParameters()
        .FirstOrDefault(p => p.ParameterType == typeof(Pageable));
    if (pageableValue is null) {
      return;
    }

    var paramNames = typeof(Pageable).GetProperties()
        .Select(p => p.Name)
        .ToHashSet();
    var toRemove = operation.Parameters
        .Where(p => paramNames.Contains(p.Name))
        .ToList();

    foreach (var index in toRemove) {
      operation.Parameters.Remove(index);
    }

    operation.Parameters.Add(new OpenApiParameter {
        Name = "page",
        In = ParameterLocation.Query,
        Description = "The page number to retrieve.",
        Required = false,
        Schema = new OpenApiSchema {
            Type = "integer",
            Format = "int32",
            Default = new OpenApiInteger(1),
            Minimum = 1
        }
    });
    operation.Parameters.Add(new OpenApiParameter {
        Name = "size",
        In = ParameterLocation.Query,
        Description = "The number of items to retrieve per page.",
        Required = false,
        Schema = new OpenApiSchema {
            Type = "integer",
            Format = "int32",
            Default = new OpenApiInteger(10),
            Minimum = 1,
            Maximum = 100
        }
    });
  }
}