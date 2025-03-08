namespace UnrealPluginManager.Core.Exceptions;

[AttributeUsage(AttributeTargets.Class)]
public class ExceptionHandlerAttribute : Attribute {
  
  public int? DefaultExitCode { get; init; }

  public string? DefaultHandlerMethod { get; init; }

}