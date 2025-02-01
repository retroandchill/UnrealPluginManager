using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;

namespace UnrealPluginManager.Server.Swagger;

/// <summary>
/// Open API type used to force the rendering of an empty object.
/// </summary>
public class ExplicitEmptyObject : IOpenApiPrimitive {
    /// <inheritdoc/>
    public AnyType AnyType => AnyType.Primitive;
    
    /// <inheritdoc/>
    public PrimitiveType PrimitiveType => PrimitiveType.String;
    
    /// <inheritdoc/>
    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion) {
        writer.WriteStartObject();
        writer.WriteEndObject();
    }
}