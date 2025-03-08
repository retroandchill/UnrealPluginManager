namespace UnrealPluginManager.ApiGenerator.Swagger;

public interface ISwaggerService {
  Task GenerateSwaggerAsync(string schemaName, string destination);
}