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
    var assemblies = new List<Type> {
            typeof(Binder)
        }
        .Concat(types)
        .Distinct()
        .Select(t => t.GetTypeInfo().Assembly.Location)
        .Select(l => MetadataReference.CreateFromFile(l));
    return CSharpCompilation.Create("compilation",
        [CSharpSyntaxTree.ParseText(source)],
        assemblies,
        new CSharpCompilationOptions(OutputKind.ConsoleApplication));
  }
}