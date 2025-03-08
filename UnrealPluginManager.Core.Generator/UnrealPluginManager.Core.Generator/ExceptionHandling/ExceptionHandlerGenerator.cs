using System;
using System.IO;
using System.Linq;
using HandlebarsDotNet;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnrealPluginManager.Core.Generator.Properties;

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
      DiagnosticSeverity.Warning,
      isEnabledByDefault: true);

  public void Initialize(IncrementalGeneratorInitializationContext context) {
    var classesProvider = context.SyntaxProvider
        .CreateSyntaxProvider((node, _) => IsValidTarget(node),
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

  private static bool IsValidTarget(SyntaxNode node) {
    return node is ClassDeclarationSyntax;
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
        .FirstOrDefault(x => x.AttributeClass?.ToDisplayString() == "UnrealPluginManager.Core.Exceptions.ExceptionHandlerAttribute");
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
    
    string? skippedHandlerMethod = null;
    var defaultExitCode = "33";
    if (arguments.TryGetValue("DefaultHandlerMethod", out var handlerMethod) && handlerMethod is string method) {
      defaultExitCode = $"{method}(ex)";
      skippedHandlerMethod = method;
    } else if (arguments.TryGetValue("DefaultExitCode", out var exitCode) && exitCode is int code) {
      defaultExitCode = $"{code}";
    }
    
    var fullData = new {
        Namespace = parentNamespace.Name.ToString(),
        ClassName = handlerClass.Identifier.Text,
        Exceptions = handlerClass.Members
            .OfType<MethodDeclarationSyntax>()
            .Where(IsExceptionHandlerMethod)
            .Where(x => x.Identifier.Text != skippedHandlerMethod)
            .Select(x => (Token: x, Symbol: semanticModel.GetDeclaredSymbol(x)!))
            .Select((x, i) => new {
                ExceptionName = x.Symbol.Parameters[0].Type.ToString(),
                VarName = $"exception{i}",
                MethodName = x.Token.Identifier.Text
            })
            .ToList(),
        DefaultExitCode = defaultExitCode
    };
    
    var template = Handlebars.Compile(Templates.ExceptionHandler);
    
    //to write our source file we can use the context object that was passed in
    //this will automatically use the path we provided in the target projects csproj file
    context.AddSource($"{handlerClass.Identifier}.g.cs", template(fullData));
  }

  private static bool IsExceptionHandlerMethod(MethodDeclarationSyntax methodDeclaration) {
    if (methodDeclaration.ReturnType.ToString() != "int") {
      return false;
    }

    var parameters = methodDeclaration.ParameterList.Parameters;
    return parameters.Count == 1;

  }
}