using System.CodeDom.Compiler;
using System.Text.RegularExpressions;
using System.Xml;
using Microsoft.CodeAnalysis;
using Mustache;
using UnrealPluginManager.Cli.Generator.Commands;
using UnrealPluginManager.Cli.Generator.Utilities;

namespace UnrealPluginManager.Cli.Generator.Generators;

[Generator]
public class CommandGenerator : IIncrementalGenerator {
    public void Initialize(IncrementalGeneratorInitializationContext context) {
        var selected = context.CompilationProvider
            .Select((c, _) => c.GlobalNamespace)
            .Select((c, _) => c.GetAllTypes()
                .Where(x => x.IsAbstract == false)
                .Where(x => x.GetAttributes()
                    .Any(a => a.AttributeClass?.Name == nameof(CommandAttribute)))
                .Where(x => x
                    .GetMembers()
                    .Any(s => s.Kind == SymbolKind.Method && s is { IsStatic: false, IsAbstract: false, Name: "Execute" }))
                .SelectMany((x, _) => {
                    var className = x.ToDisplayString();
                    var attribute = x.GetAttributes()
                        .First(a => a.AttributeClass?.Name == nameof(CommandAttribute));
                    var name = attribute.ConstructorArguments[0].Value!.ToString();
                    
                     var executeMethod = x
                         .GetMembers()
                         .Where(s => s.Kind == SymbolKind.Method && s is { IsStatic: false, IsAbstract: false, Name: "Execute" })
                         .Cast<IMethodSymbol>()
                         .First();
                     
                     var docComment = executeMethod.GetDocumentationCommentXml();
                     Dictionary<string, string> paramDocs;
                     string? description;
                     if (docComment is not null) {
                         var xmlDoc = new XmlDocument();
                         xmlDoc.LoadXml(docComment);
                         description = xmlDoc.GetElementsByTagName("summary")
                             .OfType<XmlElement>()
                             .Select(v => v.InnerText.Trim())
                             .FirstOrDefault();
                         paramDocs = xmlDoc.GetElementsByTagName("param")
                             .OfType<XmlElement>()
                             .ToDictionary(v => v.GetAttribute("name"), v => v.InnerText.Trim());
                     } else {
                         paramDocs = new Dictionary<string, string>();
                         description = null;
                     }

                     var paramNames = string.Join(", ", executeMethod.Parameters
                         .Select(p => p.Name));
                     var joinedOptions = string.Join(", ", executeMethod.Parameters
                         .Select((p, i) => $"{name}Option{i}"));

                     var parameters = executeMethod.Parameters
                         .SelectMany((p, i) => {
                             var type = p.Type.ToDisplayString();
                             var param = Regex.Replace(p.Name, "(?<!^)([A-Z])", "-$1").ToLower();
                             var docs = paramDocs.GetValueOrDefault(p.Name);
                             
                             return new [] {
                                $"var {name}Option{i} = new Option<{type}>(\"--{param}\"{(docs is not null ? $", \"{docs}\"" : "")});",
                                $"{name}Command.AddOption({name}Option{i});"
                             };
                         });

                     return new[] {
                             $"var {name} = new {className}();",
                             $"var {name}Command = new Command(\"{name}\"{(description is not null ? $", \"{description}\"" : "")});"
                         }.Concat(parameters)
                         .Concat([
                             $"{name}Command.SetHandler(({paramNames}) => {name}.Execute({paramNames}){(joinedOptions.Length > 0 ? $", {joinedOptions}" : "")});",
                             $"rootCommand.AddCommand({name}Command);"
                         ]);
                }));

        context.RegisterSourceOutput(selected, (ctx, data) => {
            var textWriter = new StringWriter();
            var writer = new IndentedTextWriter(textWriter);
            writer.WriteLine("using System.CommandLine;");
            writer.WriteLine();
            writer.WriteLine("namespace UnrealPluginManager.Cli.Utils;");
            writer.WriteLine();
            writer.WriteLine("public static partial class CommandUtils {");
            writer.Indent++;
            writer.WriteLine("public static partial void SetUpCommands(this RootCommand rootCommand) {");
            writer.Indent++;
            foreach (var line in data) {
                writer.WriteLine(line);
            }
            writer.Indent--;
            writer.WriteLine("}");
            writer.Indent--;
            writer.WriteLine("}");
            ctx.AddSource("CommandUtils.g.cs", textWriter.ToString());
        });
    }
}