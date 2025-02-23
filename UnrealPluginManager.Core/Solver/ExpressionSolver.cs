using System.Collections.Immutable;
using LanguageExt;
using Semver;
using UnrealPluginManager.Core.Database.Entities.Plugins;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Solver;

/// Static class responsible for solving logical expressions and managing variable bindings.
/// This class provides functionality to evaluate logical expressions represented by the `IExpression` interface
/// and create binding pairs of variable names and their associated `SemVersion` values.
/// It also supports converting plugin dependency data into solvable expressions.
public static class ExpressionSolver {
    /// Solves the given logical expression and returns a list of variable bindings that satisfy the expression.
    /// <param name="expr">
    /// An instance of IExpression representing the logical expression to be evaluated.
    /// </param>
    /// <returns>
    /// An Option monad containing a list of tuples, where each tuple consists of
    /// the variable name as a string and its corresponding version as a SemVersion.
    /// If no solution exists, returns None.
    /// </returns>
    public static Option<List<SelectedVersion>> Solve(this IExpression expr) {
        var selected = SolveInternal(expr, new Dictionary<string, bool>());
        return selected
            .Select(x => x.Where(y => y.Value)
                .Select(y => new SelectedVersion(VarName(y.Key), VarVersion(y.Key)))
                .ToList());
    }

    private static Option<Dictionary<string, bool>> SolveInternal(IExpression expr, Dictionary<string, bool> bindings) {
        var freeVar = AnyVar(expr);
        if (freeVar.IsNone) {
            return expr.Evaluate() ? bindings : Option<Dictionary<string, bool>>.None;
        }

        var validatedVar = (string)freeVar;
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

    /// Converts a given set of plugin data into an IExpression that represents
    /// the logical relationships and dependencies of the plugins.
    /// <param name="root">
    /// The name of the root plugin for which the logical expression is being constructed.
    /// </param>
    /// <param name="rootVersion">
    /// The version of the root plugin, represented as a SemVersion instance.
    /// </param>
    /// <param name="pluginData">
    /// A dictionary mapping plugin names to collections of plugins. Each plugin
    /// represents a specific version and contains data about its dependencies.
    /// </param>
    /// <typeparam name="T">
    /// A collection type that implements IEnumerable of Plugin, representing
    /// the structure of the plugin data values in the dictionary.
    /// </typeparam>
    /// <returns>
    /// An IExpression instance that represents all logical relationships and dependencies
    /// between the root plugin and its dependent plugins.
    /// </returns>
    public static IExpression Convert<T>(string root, SemVersion rootVersion, IDictionary<string, T> pluginData)
        where T : IEnumerable<IPluginVersionInfo> {
        List<IExpression> terms = [new Var($"{root}-v{rootVersion}")];
        foreach (var pack in
                 pluginData.Values.SelectMany(x => x.OrderBy(y => y.Version, SemVersion.PrecedenceComparer))) {
            terms.AddRange(pack.Dependencies.Where(dep => dep.Type == PluginType.Provided)
                .Select(dep => pluginData[dep.PluginName]
                    .Where(pd => dep.PluginVersion.Contains(pd.Version))
                    .Select(pd => pd.Version)
                    .Select(v => PackageVar(dep.PluginName, v))
                    .ToList())
                .Select(deps => new Impl(PackageVar(pack.Name, pack.Version), new Or(deps))));
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
                .ToHashSet();
            terms.AddRange(AllCombinations(versions)
                .Select(combo =>
                    new Not(new And([PackageVar(name, combo.Item1), PackageVar(name, combo.Item2)]))));
        }

        return new And(terms);
    }

    private static Var PackageVar(string name, SemVersion version) {
        return new Var($"{name}-v{version}");
    }

    private static string VarName(string name) {
        return name.Split("-v")[0];
    }

    private static SemVersion VarVersion(string name) {
        return SemVersion.Parse(name.Split("-v")[1]);
    }

    private static List<(SemVersion, SemVersion)> AllCombinations(IEnumerable<SemVersion> versions) {
        return versions.Combinations(2)
            .Select(x => x.ToList())
            .Select(x => (x[0], x[1]))
            .ToList();
    }
}