using Microsoft.CodeAnalysis;

namespace UnrealPluginManager.Core.Generator.Utilities;

public static class TypeExtensions {

  public static bool IsExceptionType(this ITypeSymbol type) {
    if (type.TypeKind != TypeKind.Class) {
      return false;
    }

    if (type.BaseType is null) {
      return false;
    }

    return type.ToString() == "System.Exception" || type.BaseType.IsExceptionType();

  }
  
}