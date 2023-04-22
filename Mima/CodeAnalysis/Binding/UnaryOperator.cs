namespace Mima.CodeAnalysis.Binding;

internal sealed class UnaryOperator
{
    private UnaryOperator(Syntax.Kind syntaxKind, UnaryOperatorKind kind, Type operandType)
        : this (syntaxKind, kind, operandType, operandType)
    {
    }

    private UnaryOperator(Syntax.Kind syntaxKind, UnaryOperatorKind kind, Type operandType, Type resultType)
    {
        SyntaxKind = syntaxKind;
        Kind = kind;
        OperandType = operandType;
        ResultType = resultType;
    }

    public Syntax.Kind SyntaxKind { get; }
    public UnaryOperatorKind Kind { get; }
    public Type OperandType { get; }
    public Type ResultType { get; }

    private static readonly UnaryOperator[] _operators =
    {
        new UnaryOperator(Syntax.Kind.Bang, UnaryOperatorKind.LogicalNegation, typeof(bool)),
        new UnaryOperator(Syntax.Kind.Plus, UnaryOperatorKind.Identity, typeof(int)),
        new UnaryOperator(Syntax.Kind.Minus, UnaryOperatorKind.Negation, typeof(int)),
    };

    public static UnaryOperator? Bind(Syntax.Kind kind, Type operandType)
    {
        foreach(var op in _operators)
        {
            if (op.SyntaxKind == kind && op.OperandType == operandType)
            {
                return op;
            }
        }
        
        return null;
    }
}