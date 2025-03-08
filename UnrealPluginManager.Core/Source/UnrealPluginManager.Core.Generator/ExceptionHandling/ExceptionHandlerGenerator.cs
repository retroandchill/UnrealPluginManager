﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Reflection;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnrealPluginManager.Core.Annotations.Exceptions;
using UnrealPluginManager.Core.Generator.Properties;
using UnrealPluginManager.Core.Generator.Utilities;

namespace UnrealPluginManager.Core.Generator.ExceptionHandling;

[Generator]
public class ExceptionHandlerGenerator : IIncrementalGenerator {

  private static readonly DiagnosticDescriptor RequiresPartialWarning = new(
      "UEPM001",
      "Target class must be partial", 
      "Target class '{0}' must be partial.",
      "ExceptionHandlerGenerator",
      DiagnosticSeverity.Warning, 
      isEnabledByDefault: true);

  private static readonly DiagnosticDescriptor NoNestedError = new(
      "UEPM002",
      "Target class may not be nested",
      "Target class '{0}' may not be nested.",
      "ExceptionHandlerGenerator",
      DiagnosticSeverity.Error,
      isEnabledByDefault: true);

  [SuppressMessage(
      "MicrosoftCodeAnalysisCorrectness",
      "RS1035:Do not use APIs banned for analyzers",
      Justification = "Only used here to launch the debugger correctly and not in included the release build"
  )]
  public void Initialize(IncrementalGeneratorInitializationContext context) {
    var classesProvider = context.SyntaxProvider
        .CreateSyntaxProvider((node, _) => node is ClassDeclarationSyntax,
            (ctx, _) => {
              var classDeclaration = (ClassDeclarationSyntax) ctx.Node;
              return classDeclaration;
            });

    var fullContext = context.CompilationProvider.Combine(classesProvider.Collect());
    
    context.RegisterSourceOutput(fullContext, (sourceProductionContext, combinedData) => {
      var compilation = combinedData.Left; // The Compilation instance
      var classes = combinedData.Right;   // The collected ClassDeclarationSyntax instances

      foreach (var handlerClass in classes) {
        Execute(handlerClass, compilation, sourceProductionContext);
      }

    });
  }

  private static void Execute(ClassDeclarationSyntax handlerClass,
                              Compilation compilation,
                              SourceProductionContext context) {
    var semanticModel = compilation.GetSemanticModel(handlerClass.SyntaxTree);
    var classSymbol = semanticModel.GetDeclaredSymbol(handlerClass);
    if (classSymbol is null) {
      throw new ArgumentNullException();
    }
    var attributeData = classSymbol
        .GetAttributes()
        .FirstOrDefault(x => x.IsOfAttributeType<ExceptionHandlerAttribute>());
    if (attributeData is null) {
      return;
    }
    
    if (!handlerClass.Modifiers
            .Any(m => m.IsKind(SyntaxKind.PartialKeyword))) {
      context.ReportDiagnostic(Diagnostic.Create(RequiresPartialWarning, handlerClass.Identifier.GetLocation(),
          handlerClass.Identifier.Text));
      return;
    }

    if (handlerClass.Parent is not BaseNamespaceDeclarationSyntax parentNamespace) {
      context.ReportDiagnostic(Diagnostic.Create(NoNestedError, handlerClass.Identifier.GetLocation(),
          handlerClass.Identifier.Text));
      return;
    }
    
    
    var arguments = attributeData.NamedArguments
        .ToDictionary(x => x.Key, x => x.Value.Value);
    
    var defaultExitCode = "33";
    if (arguments.TryGetValue("DefaultHandlerMethod", out var handlerMethod) && handlerMethod is string method) {
      defaultExitCode = $"{method}(ex)";
    } else if (arguments.TryGetValue("DefaultExitCode", out var exitCode) && exitCode is int code) {
      defaultExitCode = $"{code}";
    }
    
    var allMethods = handlerClass.Members
        .OfType<MethodDeclarationSyntax>()
        .Select(x => (Token: x, Symbol: semanticModel.GetDeclaredSymbol(x)!))
        .ToList();
    
    var fallbackHandlers = allMethods
        .Where(x => IsFallbackHandler(x.Symbol))
        .ToList();
    
    var dedicatedHandlers = allMethods
        .Where(x => IsSpecificHandler(x.Symbol))
        .ToList();
    
    var fullData = new {
        Namespace = parentNamespace.Name.ToString(),
        ClassName = handlerClass.Identifier.Text,
        GeneralHandlers = allMethods
            .Where(x => IsGeneralExceptionHandlerMethod(x.Symbol))
            .Select(x => new {
                x.Symbol.ReturnsVoid,
                ReturnType = x.Symbol.ReturnType.ToDisplayString(),
                Modifiers = string.Join(" ", x.Token.Modifiers.Select(m => m.ToString())),
                MethodName = x.Symbol.Name,
                ExceptionParameter = x.Symbol.Parameters[0].Name,
                Parameters = x.Symbol.Parameters
                    .Select((p, i) => new {
                        Type = p.Type.ToDisplayString(),
                        p.Name,
                        Comma = i != x.Symbol.Parameters.Length - 1
                    })
                    .ToList(),
                Exceptions = dedicatedHandlers
                    .Where(y => IsValidHandler(x.Symbol, y.Symbol))
                    .Select((y, i) => {
                      var exceptionTypes = GetExceptionTypes(y.Symbol);
                      return new {
                          ExceptionTypes = exceptionTypes
                              .Select((t, j) => new {
                                  ExceptionName = t.ToDisplayString(),
                                  Comma = j != exceptionTypes.Count - 1
                              })
                              .ToList(),
                          SingleException = exceptionTypes.Count == 1,
                          Index = i,
                          MethodName = y.Symbol.Name,
                          ExceptionType = y.Symbol.Parameters[0].Type.ToDisplayString(),
                          OtherParameters = x.Symbol.Parameters
                              .Skip(1)
                              .Take(y.Symbol.Parameters.Length - 1)
                              .Select((p, j) => new {
                                  p.Name,
                                  Comma = j != y.Symbol.Parameters.Length - 2,
                              })
                              .ToList(),
                          HasOtherParameters = x.Symbol.Parameters.Length > 1
                      };
                    })
                    .ToList(),
                FallbackHandler = fallbackHandlers
                    .Where(y => IsValidFallback(x.Symbol, y.Symbol))
                    .Select(y => new {
                        MethodName = y.Symbol.Name,
                        Parameters = x.Symbol.Parameters
                            .Take(y.Symbol.Parameters.Length)
                            .Select((p, j) => new {
                                p.Name,
                                Comma = j != y.Symbol.Parameters.Length - 1,
                            })
                            .ToList()
                    })
                    .FirstOrDefault()
            })
            .ToList()
    };
    
    var template = Handlebars.Compile(Templates.ExceptionHandler);
    
    //to write our source file we can use the context object that was passed in
    //this will automatically use the path we provided in the target projects csproj file
    context.AddSource($"{handlerClass.Identifier}.g.cs", template(fullData));
  }

  private static bool IsGeneralExceptionHandlerMethod(IMethodSymbol methodSymbol) {
    if (!methodSymbol.IsPartialDefinition) {
      return false;
    }
    
    var attribute = methodSymbol.GetAttribute<GeneralExceptionHandlerAttribute>();
    return attribute is not null && methodSymbol.Parameters.Length > 0 && methodSymbol.Parameters[0].Type.IsExceptionType();
  }

  private static bool IsFallbackHandler(IMethodSymbol methodSymbol) {
    var attribute = methodSymbol.GetAttribute<FallbackExceptionHandlerAttribute>();
    return attribute is not null && methodSymbol.Parameters.Length > 0 && methodSymbol.Parameters[0].Type.IsExceptionType();
  }
  
  private static bool IsSpecificHandler(IMethodSymbol methodSymbol) {
    var attribute = methodSymbol.GetAttribute<HandlesExceptionAttribute>();
    return attribute is not null && methodSymbol.Parameters.Length > 0 && methodSymbol.Parameters[0].Type.IsExceptionType();
  }

  private static bool IsValidHandler(IMethodSymbol parent, IMethodSymbol child) {
    if (!parent.HasCommonReturnType(child) || !parent.IsCallableFrom(child) || child.Parameters.Length > parent.Parameters.Length) {
      return false;
    }
    
    if (!GetExceptionTypes(child).All(x => x!.ConvertableTo(parent.Parameters[0].Type))) {
      return false;
    }

    for (var i = 1; i < child.Parameters.Length; i++) {
      if (!parent.Parameters[i].Type.ConvertableTo(child.Parameters[i].Type)) {
        return false;
      }
    }
    
    return true;
  }
  
  private static bool IsValidFallback(IMethodSymbol parent, IMethodSymbol child) {
    if (!parent.HasCommonReturnType(child) || !parent.IsCallableFrom(child) || child.Parameters.Length > parent.Parameters.Length) {
      return false;
    }

    return !child.Parameters.Where((t, i) => !parent.Parameters[i].Type.ConvertableTo(t.Type)).Any();
  }

  private static List<ITypeSymbol> GetExceptionTypes(IMethodSymbol method) {
    var parameterPack = method.GetAttribute<HandlesExceptionAttribute>()!.ConstructorArguments.First();
    var exceptionTypes = parameterPack.Values
        .Where(a => a.Kind == TypedConstantKind.Type)
        .Select(a => a.Value as ITypeSymbol)
        .ToList();

    if (exceptionTypes.Count == 0) {
      exceptionTypes.Add(method.Parameters[0].Type);
    }
    
    return exceptionTypes!;
  }

}