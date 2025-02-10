using Microsoft.CodeAnalysis;

namespace UnrealPluginManager.Cli.Generator.Utilities;

public static class NamespaceUtils {
    public static IEnumerable<INamedTypeSymbol> GetAllTypes(this INamespaceSymbol symbol) {
        foreach (var member in symbol.GetMembers()) {
            switch (member) {
                case INamespaceSymbol ns: {
                    foreach (var type in ns.GetAllTypes()) {
                        yield return type;
                    }

                    break;
                }
                case INamedTypeSymbol type:
                    yield return type;
                    break;
            }
        }
    }
}