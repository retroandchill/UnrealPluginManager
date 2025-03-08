using Microsoft.CodeAnalysis;

namespace UnrealPluginManager.Core.Generator.Utilities;

public static class MethodExtensions {

  public static bool HasCommonReturnType(this IMethodSymbol method, IMethodSymbol otherMethod) {
    if (method.ReturnsVoid) {
      return true;
    }

    return !otherMethod.ReturnsVoid && otherMethod.ReturnType.ConvertableTo(method.ReturnType);
  }
  
  public static bool IsCallableFrom(this IMethodSymbol method, IMethodSymbol otherMethod) {
    return otherMethod.IsStatic || method.IsStatic == otherMethod.IsStatic;
  }
  
}