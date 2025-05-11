using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using UnrealPluginManager.Core.Model.Plugins.Recipes;
using UnrealPluginManager.Server.Controllers;

namespace UnrealPluginManager.ApiGenerator.Swagger;

public class PluginSubmissionOperationFilter : IOperationFilter {
  public void Apply(OpenApiOperation operation, OperationFilterContext context) {
    if (context.MethodInfo.Name == nameof(PluginsController.SubmitPlugin)) {
      operation.RequestBody = new OpenApiRequestBody {
          Content = new Dictionary<string, OpenApiMediaType> {
              ["multipart/form-data"] = new() {
                  Schema = new OpenApiSchema {
                      Type = "object",
                      Properties = new Dictionary<string, OpenApiSchema> {
                          ["manifest"] = new() {
                              Reference = new OpenApiReference {
                                  Type = ReferenceType.Schema,
                                  Id = nameof(PluginManifest)
                              }
                          },
                          ["patches"] = new() {
                              Type = "array",
                              Items = new OpenApiSchema {
                                  Type = "string"
                              },
                              Nullable = true
                          },
                          ["icon"] = new() {
                              Type = "string",
                              Format = "binary",
                              Nullable = true
                          },
                          ["readme"] = new() {
                              Type = "string",
                              Nullable = true
                          }
                      },
                      Required = new HashSet<string> {
                          "manifest"
                      }
                  },
                  Encoding = new Dictionary<string, OpenApiEncoding> {
                      ["manifest"] = new() {
                          ContentType = "application/json"
                      },
                      ["patches"] = new() {
                          ContentType = "application/json"
                      },
                      ["icon"] = new() {
                          ContentType = "image/png"
                      },
                      ["readme"] = new() {
                          ContentType = "text/markdown"
                      }
                  }
              }
          }
      };
    }
  }
}