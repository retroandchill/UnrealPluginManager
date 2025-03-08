using Microsoft.OpenApi.Models;
using Semver;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace UnrealPluginManager.ApiGenerator.Swagger;

/// <summary>
/// Represents an operation filter that applies a specific schema pattern
/// for parameters of type <c>SemVersion</c> to enforce semantic versioning format.
/// </summary>
/// <remarks>
/// This filter is used to ensure that parameters which are of type <c>SemVersion</c>
/// adhere to a specific regular expression pattern for semantic versioning.
/// </remarks>
/// <example>
/// This operation filter modifies the schema of path parameters matching type <c>SemVersion</c>
/// by setting a regex pattern defined in the <c>SwaggerExtensions.SemVerPattern</c>.
/// The pattern is applied only for matching path parameters in an OpenAPI operation.
/// </example>
public class SemVersionParameterFilter : IOperationFilter {
  
  /// <inheritdoc />
  public void Apply(OpenApiOperation operation, OperationFilterContext context) {
    foreach (var param in context.MethodInfo.GetParameters()
                 .Where(p => p.ParameterType == typeof(SemVersion))) {
      var pathParam = operation.Parameters
          .FirstOrDefault(p => p.Name == param.Name && p.In == ParameterLocation.Path);
      if (pathParam is null) {
        continue;
      }

      pathParam.Schema.Pattern = SwaggerExtensions.SemVerPattern;
    }
  }
}