﻿using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using UnrealPluginManager.Core.Pagination;

namespace UnrealPluginManager.Server.Swagger;

/// <summary>
/// The <c>PagePropertyFilter</c> class defines a schema filter for modifying the OpenAPI schema of generic
/// types based on the <c>Page&lt;T&gt;</c> class. This filter customizes the OpenAPI description
/// of paged results by including detailed metadata properties.
/// </summary>
/// <remarks>
/// When applied to a generic type based on the <c>Page&lt;T&gt;</c> structure, this filter modifies the OpenAPI schema
/// to represent the data structure as an object containing the following properties:
/// - <c>pageNumber</c>: Represents the current page number of the result set. It must be a positive integer.
/// - <c>totalPages</c>: Represents the total number of pages in the result set. It must be a positive integer.
/// - <c>pageSize</c>: Represents the number of items on each page. It must be a positive integer with an example value of 10.
/// - <c>count</c>: Represents the total number of items available in the result set. It must be a non-negative integer with an example value of 1.
/// - <c>item</c>: Represents an array of items on the current page.
/// This process removes the schema's default item definition and restructures the type as described above.
/// </remarks>
public class PagePropertyFilter : ISchemaFilter {

    /// <inheritdoc />
    public void Apply(OpenApiSchema schema, SchemaFilterContext context) {
        if (!context.Type.IsGenericType || context.Type.GetGenericTypeDefinition() != typeof(Page<>)) {
            return;
        }
        
        
        schema.Type = "object";
        schema.Properties = new Dictionary<string, OpenApiSchema>();
        schema.Properties.Add("pageNumber", new OpenApiSchema {
            Type = "integer",
            Format = "int32",
            Minimum = 1
        });
        schema.Properties.Add("totalPages", new OpenApiSchema {
            Type = "integer",
            Format = "int32",
            Minimum = 1
        });
        schema.Properties.Add("pageSize", new OpenApiSchema {
            Type = "integer",
            Format = "int32",
            Minimum = 1,
            Example = new OpenApiInteger(10)
        });
        schema.Properties.Add("count", new OpenApiSchema {
            Type = "integer",
            Format = "int32",
            Minimum = 0,
            Example = new OpenApiInteger(1)
        });
        schema.Properties.Add("item", new OpenApiSchema {
            Type = "array",
            Items = schema.Items
        });
        
        schema.Items = null;
    }
}