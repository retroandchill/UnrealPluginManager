using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using UnrealPluginManager.Core.Exceptions;

namespace UnrealPluginManager.Core.Generator.Tests.Helpers;

public static class GeneratorTestHelpers {
  public static Compilation CreateCompilation(string source, params Type[] types) {
    var assemblies = AppDomain.CurrentDomain.GetAssemblies()
        .Select(a => a.Location)
        .Where(a => !string.IsNullOrEmpty(a))
        .Select(l => MetadataReference.CreateFromFile(l));
    return CSharpCompilation.Create("compilation",
        [CSharpSyntaxTree.ParseText(source)],
        assemblies,
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
  }
}