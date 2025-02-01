using Microsoft.OpenApi;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Writers;

namespace UnrealPluginManager.Server.Swagger;

/// <summary>
/// Open API type used to force the rendering of an empty array.
/// </summary>
public class ExplictEmptyArray : IOpenApiPrimitive {
    /// <inheritdoc/>
    public AnyType AnyType => AnyType.Primitive;
    
    /// <inheritdoc/>
    public PrimitiveType PrimitiveType => PrimitiveType.String;
    
    /// <inheritdoc/>
    public void Write(IOpenApiWriter writer, OpenApiSpecVersion specVersion) {
        writer.WriteStartArray();
        writer.WriteEndArray();
    }
    
}