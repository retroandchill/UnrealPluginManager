using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace UnrealPluginManager.Core.Generator.Utilities;

public static class AttributeExtensions {

  public static bool IsOfAttributeType<T>(this AttributeData attribute) where T : Attribute {
    return attribute.AttributeClass?.ToString() == typeof(T).FullName;
  }
  
  public static AttributeData? GetAttribute<T>(this ISymbol symbol) where T : Attribute {
    return symbol.GetAttributes()
        .FirstOrDefault(a => a.IsOfAttributeType<T>());
  }

}