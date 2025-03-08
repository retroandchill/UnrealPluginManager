using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;
using UnrealPluginManager.Core.Annotations.Exceptions;
using UnrealPluginManager.Core.Exceptions;
using UnrealPluginManager.Core.Generator.ExceptionHandling;
using static UnrealPluginManager.Core.Generator.Tests.Helpers.GeneratorTestHelpers;

namespace UnrealPluginManager.Core.Generator.Tests.ExceptionHandling;

public class CreateExceptionHandlerTest {
  
  [Test]
  public void TestCreateExceptionHandler() {
    var inputCompilation = CreateCompilation("""
                                             using System;
                                             using UnrealPluginManager.Core.Exceptions;
                                             using UnrealPluginManager.Core.Annotations.Exceptions;

                                             namespace Test;

                                             [ExceptionHandler]
                                             public partial class TestHandler {
                                               [GeneralExceptionHandler]
                                               public partial int HandleException(Exception ex);
                                             
                                               [HandlesException]
                                               public int HandleSingle(PluginNotFoundException ex) {
                                                 return 4;
                                               }
                                               
                                                [HandlesException(typeof(DependencyConflictException, typeof(MissingDependenciesException)))]
                                                public int HandleMultiple(Exception ex) {
                                                  return 4;
                                                }
                                             }

                                             """, typeof(ExceptionHandlerAttribute), typeof(PluginNotFoundException));

    var generator = new ExceptionHandlerGenerator();
    
    var driver = CSharpGeneratorDriver.Create(generator);
    driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
    Assert.That(diagnostics.Length, Is.EqualTo(0));
  }
  
  [Test]
  public void TestCreateExceptionHandler_NoPartialClass() {
    var inputCompilation = CreateCompilation("""
                                             using System.Exception;
                                             using UnrealPluginManager.Core.Exceptions;
                                             using UnrealPluginManager.Core.Annotations.Exceptions;

                                             namespace Test;

                                             [ExceptionHandler]
                                             public class TestHandler {
                                               public int HandleException(PluginNotFoundException ex) {
                                                 return 4;
                                               }
                                             }

                                             """, typeof(ExceptionHandlerAttribute), typeof(PluginNotFoundException));

    var generator = new ExceptionHandlerGenerator();
    
    var driver = CSharpGeneratorDriver.Create(generator);
    driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
    Assert.That(diagnostics.Length, Is.EqualTo(1));
    
    Assert.That(diagnostics[0].Id, Is.EqualTo("UEPM001"));
    Assert.That(diagnostics[0].Severity, Is.EqualTo(DiagnosticSeverity.Warning));
  }
  
  [Test]
  public void TestCreateExceptionHandler_NestedClass() {
    var inputCompilation = CreateCompilation("""
                                             using System.Exception;
                                             using UnrealPluginManager.Core.Exceptions;
                                             using UnrealPluginManager.Core.Annotations.Exceptions;

                                             namespace Test;

                                             
                                             public partial class TestHandler {
                                               [ExceptionHandler]
                                               public partial class NestedHandler {
                                                 public int HandleException(PluginNotFoundException ex) {
                                                   return 4;
                                                 }
                                               }
                                             }

                                             """, typeof(ExceptionHandlerAttribute), typeof(PluginNotFoundException));

    var generator = new ExceptionHandlerGenerator();
    
    var driver = CSharpGeneratorDriver.Create(generator);
    driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
    Assert.That(diagnostics.Length, Is.EqualTo(1));
    
    Assert.That(diagnostics[0].Id, Is.EqualTo("UEPM002"));
    Assert.That(diagnostics[0].Severity, Is.EqualTo(DiagnosticSeverity.Error));
  }

  
}