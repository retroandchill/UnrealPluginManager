using System.Collections.Immutable;
using LanguageExt;

namespace UnrealPluginManager.Core.Solver;

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
    /// <returns>
    /// A boolean value indicating the result of the evaluation.
    /// Throws an exception if the expression contains free variables that cannot be evaluated.
    /// </returns>
    bool Evaluate();

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

    /// <inheritdoc/>
    public bool Evaluate() {
        return Value;
    }

    /// <inheritdoc/>
    public IExpression Replace(SelectedVersion varName, bool varValue) {
        return new BoolExpression(Value);
    }
}

/// <summary>
/// Represents a variable in a logical expression. A variable holds a name and can be replaced or evaluated within the expression context.
/// </summary>
public record Var(SelectedVersion Name) : IExpression {
    /// <inheritdoc/>
    public IEnumerable<SelectedVersion> Free() {
        return [Name];
    }

    /// <inheritdoc/>
    public bool Evaluate() {
        throw new NotSupportedException($"The variable {Name} has not been replaced");
    }

    /// <inheritdoc/>
    public IExpression Replace(SelectedVersion varName, bool varValue) {
        if (Name == varName) {
            return new BoolExpression(varValue);
        }

        return new Var(Name);
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

    /// <inheritdoc/>
    public bool Evaluate() {
        return !Expression.Evaluate();
    }

    /// <inheritdoc/>
    public IExpression Replace(SelectedVersion varName, bool varValue) {
        return new Not(Expression.Replace(varName, varValue));
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

    /// <inheritdoc/>
    public bool Evaluate() {
        return Expressions.All(e => e.Evaluate());
    }

    /// <inheritdoc/>
    public IExpression Replace(SelectedVersion varName, bool varValue) {
        return new And(Expressions.Select(x => x.Replace(varName, varValue)).ToList());
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
    public bool Evaluate() {
        return Expressions.Any(e => e.Evaluate());
    }

    /// <inheritdoc/>
    public IExpression Replace(SelectedVersion varName, bool varValue) {
        return new Or(Expressions.Select(x => x.Replace(varName, varValue)).ToList());
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
public record Impl(IExpression P, IExpression Q) : IExpression {
    /// <inheritdoc/>
    public IEnumerable<SelectedVersion> Free() {
        return P.Free().Concat(Q.Free()).ToImmutableSortedSet();
    }

    /// <inheritdoc/>
    public bool Evaluate() {
        return !P.Evaluate() || Q.Evaluate();
    }

    /// <inheritdoc/>
    public IExpression Replace(SelectedVersion varName, bool varValue) {
        return new Impl(P.Replace(varName, varValue), Q.Replace(varName, varValue));
    }
}