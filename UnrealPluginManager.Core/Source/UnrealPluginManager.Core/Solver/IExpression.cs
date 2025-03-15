using System.Collections.Immutable;
using LanguageExt;
using UnrealPluginManager.Core.Model.Plugins;
using UnrealPluginManager.Core.Model.Resolution;

namespace UnrealPluginManager.Core.Solver;


/// <summary>
/// Represents the result of an evaluation process with information about its outcome, 
/// the source of the requirement, and the specific plugin dependency involved.
/// </summary>
/// <param name="Result">
/// A boolean value indicating the result of the evaluation:
/// <c>true</c> if the evaluation succeeded, and <c>false</c> otherwise.
/// </param>
/// <param name="RequiredBy">
/// A string that specifies the entity or component requiring the evaluation. 
/// This provides traceability for understanding which part of the system 
/// or dependency chain is involved.
/// </param>
/// <param name="Dependency">
/// The plugin dependency involved in the evaluation. 
/// This captures details about the specific plugin or library related to the evaluation result.
/// </param>
/// <remarks>
/// The <see cref="EvaluationResult"/> is commonly used in the context of evaluating 
/// logical expressions and plugin dependencies. It standardizes the output format 
/// for evaluations performed during plugin resolution workflows.
/// </remarks>
public record struct EvaluationResult(bool Result, string RequiredBy, PluginDependency Dependency);

/// <summary>
/// Represents a logical expression interface that defines methods for evaluating and manipulating logical expressions.
/// </summary>
public interface IExpression {
  /// <summary>
  /// Retrieves the set of free variables contained within the logical expression.
  /// A free variable is one that has not been bound or replaced with a specific value.
  /// </summary>
  /// <returns>
  /// An enumerable collection of variable names that are free within the expression.
  /// If no free variables exist, an empty collection is returned.
  /// </returns>
  IEnumerable<SelectedVersion> Free();

  /// <summary>
  /// Evaluates the logical expression to determine its truth value.
  /// </summary>
  /// <param name="results"></param>
  /// <returns>
  /// A boolean value indicating the result of the evaluation.
  /// Throws an exception if the expression contains free variables that cannot be evaluated.
  /// </returns>
  bool Evaluate(List<EvaluationResult> results);

  /// <summary>
  /// Replaces every occurrence of a specified variable within the logical expression
  /// with the corresponding boolean value.
  /// </summary>
  /// <param name="varName">
  /// The name of the variable to be replaced in the expression.
  /// </param>
  /// <param name="varValue">
  /// The boolean value that will replace the occurrences of the specified variable.
  /// </param>
  /// <returns>
  /// A new logical expression where all occurrences of the specified variable are replaced
  /// with the provided boolean value. If the variable is not present in the expression, the
  /// original expression is returned unchanged.
  /// </returns>
  IExpression Replace(SelectedVersion varName, bool varValue);
}

/// <summary>
/// Represents a simple boolean expression that implements the logical expression interface.
/// This expression encapsulates a constant boolean value which can be evaluated directly.
/// </summary>
public record BoolExpression(bool Value) : IExpression {
  /// <inheritdoc/>
  public IEnumerable<SelectedVersion> Free() {
    return [];
  }

  /// <param name="results"></param>
  /// <inheritdoc/>
  public bool Evaluate(List<EvaluationResult> results) {
    return Value;
  }

  /// <inheritdoc/>
  public IExpression Replace(SelectedVersion varName, bool varValue) {
    return new BoolExpression(Value);
  }

  /// <inheritdoc />
  public override string ToString() {
    return Value.ToString();
  }
}

/// <summary>
/// Represents a variable in a logical expression. A variable holds a name and can be replaced or evaluated within the expression context.
/// </summary>
public record Var(SelectedVersion Selected) : IExpression {
  /// <inheritdoc/>
  public IEnumerable<SelectedVersion> Free() {
    return [Selected];
  }

  /// <param name="results"></param>
  /// <inheritdoc/>
  public bool Evaluate(List<EvaluationResult> results) {
    throw new NotSupportedException($"The variable {Selected} has not been replaced");
  }

  /// <inheritdoc/>
  public IExpression Replace(SelectedVersion varName, bool varValue) {
    if (Selected == varName) {
      return new BoolExpression(varValue);
    }

    // Plugin versions are mutually exclusive, so we want to assign all others with the same name to false.
    // This speeds up the algorithm a bit
    if (Selected.Name == varName.Name && varValue) {
      return new BoolExpression(false);
    }

    return new Var(Selected);
  }

  /// <inheritdoc />
  public override string ToString() {
    return $"{Selected.Name}/{Selected.Version}";
  }
}

/// <summary>
/// Represents a logical NOT operation in a boolean expression. This expression negates the result of its underlying expression.
/// </summary>
public record Not(IExpression Expression) : IExpression {
  /// <inheritdoc/>
  public IEnumerable<SelectedVersion> Free() {
    return Expression.Free();
  }

  /// <param name="results"></param>
  /// <inheritdoc/>
  public bool Evaluate(List<EvaluationResult> results) {
    return !Expression.Evaluate(results);
  }

  /// <inheritdoc/>
  public IExpression Replace(SelectedVersion varName, bool varValue) {
    return new Not(Expression.Replace(varName, varValue));
  }

  /// <inheritdoc />
  public override string ToString() {
    return $"!{Expression}";
  }
}

/// <summary>
/// Represents a logical "AND" operation applied to a collection of logical expressions.
/// </summary>
public record And(IEnumerable<IExpression> Expressions) : IExpression {
  /// <inheritdoc/>
  public IEnumerable<SelectedVersion> Free() {
    return Expressions.SelectMany(e => e.Free()).ToImmutableSortedSet();
  }

  /// <param name="results"></param>
  /// <inheritdoc/>
  public bool Evaluate(List<EvaluationResult> results) {
    return Expressions.All(e => e.Evaluate(results));
  }

  /// <inheritdoc/>
  public IExpression Replace(SelectedVersion varName, bool varValue) {
    return new And(Expressions.Select(x => x.Replace(varName, varValue)).ToList());
  }

  /// <inheritdoc />
  public override string ToString() {
    return $"({string.Join(" && ", Expressions.Select(x => x.ToString()))})";
  }
}

/// <summary>
/// Represents a logical expression that evaluates to true if any of its contained expressions evaluate to true.
/// </summary>
public record Or(IEnumerable<IExpression> Expressions) : IExpression {
  /// <inheritdoc/>
  public IEnumerable<SelectedVersion> Free() {
    return Expressions.SelectMany(e => e.Free()).ToImmutableSortedSet();
  }

  /// <inheritdoc/>
  public bool Evaluate(List<EvaluationResult> results) {
    return Expressions.Any(e => e.Evaluate(results));
  }

  /// <inheritdoc/>
  public IExpression Replace(SelectedVersion varName, bool varValue) {
    return new Or(Expressions.Select(x => x.Replace(varName, varValue)).ToList());
  }

  /// <inheritdoc />
  public override string ToString() {
    return $"({string.Join(" || ", Expressions.Select(x => x.ToString()))})";
  }
}

/// <summary>
/// Represents an implication expression consisting of two sub-expressions, P and Q, following the logical formula "if P, then Q".
/// </summary>
/// <remarks>
/// The Impl class models the logical implication operation, where the evaluation of the expression returns true
/// if the first sub-expression (P) is false or the second sub-expression (Q) is true. It also provides methods
/// to retrieve free variables and replace variables within the expression.
/// </remarks>
public record Impl(IExpression P, IExpression Q, string RequiredBy, PluginDependency Dependency) : IExpression {
  /// <inheritdoc/>
  public IEnumerable<SelectedVersion> Free() {
    return P.Free().Concat(Q.Free()).ToImmutableSortedSet();
  }

  /// <param name="results"></param>
  /// <inheritdoc/>
  public bool Evaluate(List<EvaluationResult> results) {
    if (!P.Evaluate(results)) {
      return true;
    }

    var depEvaluation = Q.Evaluate(results);
    results.Add(new EvaluationResult(depEvaluation, RequiredBy, Dependency));
    return depEvaluation;
  }

  /// <inheritdoc/>
  public IExpression Replace(SelectedVersion varName, bool varValue) {
    return this with { P = P.Replace(varName, varValue), Q = Q.Replace(varName, varValue) };
  }

  /// <inheritdoc />
  public override string ToString() {
    return $"{P} => {Q}";
  }
}
