using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace UnrealPluginManager.Core.Generator.Utilities;

public static class TypeExtensions {

  public static bool IsExceptionType(this ITypeSymbol type) {
    return type.IsOfType<Exception>();
  }

  public static bool IsOfType<T>(this ITypeSymbol type) {
    if (type.ToString() == typeof(T).FullName) {
      return true;
    }

    if (typeof(T).IsClass && type is { TypeKind: TypeKind.Class, BaseType: not null }) {
      return type.BaseType.IsOfType<T>();
    }
    
    if (typeof(T).IsInterface && type.TypeKind is TypeKind.Interface or TypeKind.Class) {
      return type.Interfaces
          .Any(i => i.IsOfType<T>());
    }
    
    return false;
  }

  public static bool IsOfType(this ITypeSymbol type, ITypeSymbol other) {
    if (type.ToString() == other.ToString()) {
      return true;
    }

    return other.TypeKind switch {
        TypeKind.Class when type is { TypeKind: TypeKind.Class, BaseType: not null } => type.BaseType.IsOfType(other),
        TypeKind.Interface when type.TypeKind is TypeKind.Interface or TypeKind.Class => type.Interfaces
            .Any(i => i.IsOfType(other)),
        _ => false
    };

  }
  
  public static bool ConvertableTo(this ITypeSymbol type, ITypeSymbol other) {
    if (type.IsOfType(other)) {
      return true;
    }

    return false;
  }
  
}