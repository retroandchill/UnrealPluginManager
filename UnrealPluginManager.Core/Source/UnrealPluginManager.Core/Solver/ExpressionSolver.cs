using LanguageExt;
using Semver;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Resolution;
using UnrealPluginManager.Core.Utils;

namespace UnrealPluginManager.Core.Solver;

using SolveResult = Either<List<SelectedVersion>, List<Conflict>>;

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
  public static SolveResult Solve(this IExpression expr) {
    var conflicts = new List<Conflict>();
    var selected = SolveInternal(expr, new Dictionary<SelectedVersion, bool>(), conflicts);
    return selected
        .Match(SolveResult (x) => x.Where(y => y.Value)
                .Select(y => y.Key)
                .ToList(),
            () => conflicts);
  }

  private static Option<Dictionary<SelectedVersion, bool>> SolveInternal(
      IExpression expr, Dictionary<SelectedVersion, bool> bindings, List<Conflict> conflicts) {
    var freeVar = AnyVar(expr);
    if (freeVar.IsNone) {
      var results = new List<EvaluationResult>();
      if (expr.Evaluate(results)) {
        return bindings;
      }

      conflicts.AddRange(results.GroupBy(x => x.Dependency.PluginName)
          .Where(x => x.Count() > 1)
          .Where(x => x.Any(y => !y.Result))
          .Select(x =>
              new Conflict(x.Key, x.Select(y =>
                      new PluginRequirement(y.RequiredBy,
                          y.Dependency.PluginVersion))
                  .ToList())));

      return Option<Dictionary<SelectedVersion, bool>>.None;
    }

    var validatedVar = (SelectedVersion) freeVar;
    var trueExpr = expr.Replace(validatedVar, true);
    var trueBindings = new Dictionary<SelectedVersion, bool>(bindings);
    trueBindings[validatedVar] = true;

    var falseExpr = expr.Replace(validatedVar, false);
    var falseBindings = new Dictionary<SelectedVersion, bool>(bindings);
    falseBindings[validatedVar] = false;

    return SolveInternal(trueExpr, trueBindings, conflicts) || SolveInternal(falseExpr, falseBindings, conflicts);
  }

  private static Option<SelectedVersion> AnyVar(IExpression expr) {
    var variables = expr.Free().OrderByDescending(x => x).ToList();
    return variables.Count != 0 ? variables[0] : Option<SelectedVersion>.None;
  }

  /// Converts a given set of plugin data into an IExpression that represents
  /// the logical relationships and dependencies of the plugins.
  /// <param name="node"></param>
  /// <param name="pluginData">
  ///     A dictionary mapping plugin names to collections of plugins. Each plugin
  ///     represents a specific version and contains data about its dependencies.
  /// </param>
  /// <typeparam name="T">
  /// A collection type that implements IEnumerable of Plugin, representing
  /// the structure of the plugin data values in the dictionary.
  /// </typeparam>
  /// <returns>
  /// An IExpression instance that represents all logical relationships and dependencies
  /// between the root plugin and its dependent plugins.
  /// </returns>
  public static IExpression Convert<T>(IDependencyChainNode node, IDictionary<string, T> pluginData)
      where T : IEnumerable<IDependencyChainNode> {
    List<IExpression> terms = [
        new Var(new SelectedVersion(node.Name, node.Version) {
            Installed = node.Installed,
            RemoteIndex = node.RemoteIndex
        })
    ];

    var dependencyFrequency = node.Dependencies
        .Concat(pluginData.SelectMany(x => x.Value)
            .SelectMany(x => x.Dependencies))
        .GroupBy(x => x.PluginName)
        .Select(x => (x.Key, x.Count()))
        .OrderByDescending(x => x.Item2)
        .Select(x => x.Item1)
        .ToList();

    foreach (var pack in node.ToEnumerable()
                 .Concat(dependencyFrequency.Select(x => pluginData[x])
                     .SelectMany(x => x.OrderBy(y => y.Version, SemVersion.PrecedenceComparer)))) {
      terms.AddRange(pack.Dependencies.Select(dep => (Dep: dep, Vars: pluginData[dep.PluginName]
              .Where(pd => dep.PluginVersion.Contains(pd.Version))
              .Select(pd => PackageVar(dep.PluginName, pd.Version, pd.Installed, pd.RemoteIndex))
              .ToList()))
          .Select(deps => new Impl(PackageVar(pack.Name, pack.Version, pack.Installed, pack.RemoteIndex),
              new Or(deps.Vars), pack.Name, deps.Dep)));
    }

    var variables = new And(terms).Free()
        .ToHashSet();
    var varNames = variables
        .Select(x => x.Name)
        .ToHashSet();
    foreach (var versions in varNames.Select(name => variables
                 .Where(v => v.Name == name)
                 .ToHashSet())) {
      terms.AddRange(AllCombinations(versions)
          .Select(combo =>
              new Not(new And([PackageVar(combo.Item1), PackageVar(combo.Item2)]))));
    }

    return new And(terms);
  }

  private static Var PackageVar(string name, SemVersion version, bool installed, int? remoteIndex) {
    return PackageVar(new SelectedVersion(name, version) {
        Installed = installed,
        RemoteIndex = remoteIndex
    });
  }

  private static Var PackageVar(SelectedVersion version) {
    return new Var(version);
  }

  private static List<(SelectedVersion, SelectedVersion)> AllCombinations(IEnumerable<SelectedVersion> versions) {
    return versions.Combinations(2)
        .Select(x => x.ToList())
        .Select(x => (x[0], x[1]))
        .ToList();
  }
}