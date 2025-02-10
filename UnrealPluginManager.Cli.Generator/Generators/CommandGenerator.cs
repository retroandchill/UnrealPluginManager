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
                .Select((x, _) => {
                    var className = x.ToDisplayString();
                    var attribute = x.GetAttributes()
                        .First(a => a.AttributeClass?.Name == nameof(CommandAttribute));
                    var name = attribute.ConstructorArguments[0].Value!.ToString();
                    var description = attribute.ConstructorArguments[1].Value?.ToString();

                    return new List<string> {
                        $"var {name} = new {className}();",
                        $"var {name}Command = new Command(\"{name}\", \"{description ?? ""}\");",
                        $"{name}Command.SetHandler(() => {name}.Execute());",
                        $"rootCommand.AddCommand({name}Command);"
                    };
                }));
        
        context.RegisterSourceOutput(selected, (ctx, data) => {
            var start = """
                           using System.CommandLine;

                           namespace UnrealPluginManager.Cli.Utils;

                           public  static partial class CommandUtils {

                            public static partial void SetUpCommands(this RootCommand rootCommand) {
                           """;

            var end = "} }";
            var result = string.Join('\n', data.SelectMany(x => x));
            ctx.AddSource("CommandUtils.g.cs", $"{start}{result}{end}");
        });
    }
}