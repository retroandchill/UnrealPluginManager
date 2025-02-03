using System.Collections.Immutable;
using LanguageExt;

namespace UnrealPluginManager.Core.Solver;

public interface IExpression {
    IEnumerable<string> Free();

    bool Evaluate();

    IExpression Replace(string varName, bool varValue);
}

public record BoolExpression(bool Value) : IExpression {
    public IEnumerable<string> Free() {
        return [];
    }

    public bool Evaluate() {
        return Value;
    }

    public IExpression Replace(string varName, bool varValue) {
        return new BoolExpression(Value);
    }
}

public record Var(string Name) : IExpression {
    public IEnumerable<string> Free() {
        return [Name];
    }

    public bool Evaluate() {
        throw new NotSupportedException($"The variable {Name} has not been replaced");
    }

    public IExpression Replace(string varName, bool varValue) {
        if (Name == varName) {
            return new BoolExpression(varValue);
        }

        return new Var(Name);
    }
}

public record Not(IExpression Expression) : IExpression {
    public IEnumerable<string> Free() {
        return Expression.Free();
    }

    public bool Evaluate() {
        return !Expression.Evaluate();
    }

    public IExpression Replace(string varName, bool varValue) {
        return new Not(Expression.Replace(varName, varValue));
    }
}

public record And(IEnumerable<IExpression> Expressions) : IExpression {
    public IEnumerable<string> Free() {
        return Expressions.SelectMany(e => e.Free()).ToImmutableSortedSet();
    }

    public bool Evaluate() {
        return Expressions.All(e => e.Evaluate());
    }

    public IExpression Replace(string varName, bool varValue) {
        return new And(Expressions.Select(x => x.Replace(varName, varValue)).ToList());
    }
}

public record Or(IEnumerable<IExpression> Expressions) : IExpression {
    public IEnumerable<string> Free() {
        return Expressions.SelectMany(e => e.Free()).ToImmutableSortedSet();
    }

    public bool Evaluate() {
        return Expressions.Any(e => e.Evaluate());
    }

    public IExpression Replace(string varName, bool varValue) {
        return new Or(Expressions.Select(x => x.Replace(varName, varValue)).ToList());
    }
}

public record Impl(IExpression P, IExpression Q) : IExpression {
    public IEnumerable<string> Free() {
        return P.Free().Concat(Q.Free()).ToImmutableSortedSet();
    }

    public bool Evaluate() {
        return !P.Evaluate() || Q.Evaluate();
    }

    public IExpression Replace(string varName, bool varValue) {
        return new Impl(P.Replace(varName, varValue), Q.Replace(varName, varValue));
    }
}