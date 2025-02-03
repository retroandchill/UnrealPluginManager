using System.Collections.Immutable;
using LanguageExt;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Solver;

public static class ExpressionSolver {
    public static Option<List<(string, Version)>> Solve(this IExpression expr) {
        var selected = SolveInternal(expr, new Dictionary<string, bool>());
        return selected
            .Select(x => x.Where(y => y.Value)
                .Select(y => (VarName(y.Key), VarVersion(y.Key)))
                .ToList());
    }

    private static Option<Dictionary<string, bool>> SolveInternal(IExpression expr, Dictionary<string, bool> bindings) {
        var freeVar = AnyVar(expr);
        if (freeVar.IsNone) {
            if (expr.Evaluate()) {
                return bindings;
            }
            return Option<Dictionary<string, bool>>.None;
        }

        var validatedVar = (string) freeVar;
        var trueExpr = expr.Replace(validatedVar, true);
        var trueBindings = new Dictionary<string, bool>(bindings);
        trueBindings[validatedVar] = true;
        
        var falseExpr = expr.Replace(validatedVar, false);
        var falseBindings = new Dictionary<string, bool>(bindings);
        falseBindings[validatedVar] = false;

        return SolveInternal(trueExpr, trueBindings) || SolveInternal(falseExpr, falseBindings);
    }

    private static Option<string> AnyVar(IExpression expr) {
        var variables = expr.Free().OrderByDescending(x => x).ToList();
        return variables.Count != 0 ? variables[0] : Option<string>.None;
    }

    public static IExpression Convert<T>(string root, Version rootVersion, IDictionary<string, T> pluginData) where T : IEnumerable<Plugin> {
        List<IExpression> terms = [new Var($"{root}-v{rootVersion}")];
        foreach (var pack in pluginData.Values.SelectMany(x => x.OrderBy(y => y.Version))) {
            foreach (var dep in pack.Dependencies) {
                var versions =  pluginData[dep.PluginName]
                    .Where(pd => dep.PluginVersion.Contains(pd.Version.ToSemVersion()))
                    .Select(pd => pd.Version)
                    .ToList();
                var deps = versions
                    .Select(v => PackageVar(dep.PluginName, v))
                    .ToList();
                var impl = new Impl(PackageVar(pack.Name, pack.Version), new Or(deps));
                terms.Add(impl);
            }
        }

        var variables = new And(terms).Free()
            .ToHashSet();
        var varNames = variables
            .Select(VarName)
            .ToHashSet();
        foreach (var name in varNames) {
            var versions = variables
                .Where(v => VarName(v) == name)
                .Select(VarVersion)
                .ToImmutableSortedSet();
            terms.AddRange(AllCombinations(versions)
                .Select(combo => 
                    new Not(new And([PackageVar(name, combo.Item1), PackageVar(name, combo.Item2)]))));
        }

        return new And(terms);
    }
    
    private static Var PackageVar(string name, Version version) {
        return new Var($"{name}-v{version}");
    }

    private static string VarName(string name) {
        return name.Split("-v")[0];
    }

    private static Version VarVersion(string name) {
        return Version.Parse(name.Split("-v")[1]);
    }

    private static ISet<(Version, Version)> AllCombinations(IEnumerable<Version> versions) {
        return versions.Combinations(2)
            .Select(x => x.ToList())
            .Select(x => (x[0], x[1]))
            .ToImmutableSortedSet();
    }
    
    
}