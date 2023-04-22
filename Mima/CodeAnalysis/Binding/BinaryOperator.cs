namespace Mima.CodeAnalysis.Binding;

internal sealed class BinaryOperator
{
    private BinaryOperator(Syntax.Kind syntaxKind, BinaryOperatorKind kind, Type type)
        : this(syntaxKind, kind, type, type, type)
    {
    }

    private BinaryOperator(Syntax.Kind syntaxKind, BinaryOperatorKind kind, Type operandType, Type resultType)
: this(syntaxKind, kind, operandType, operandType, resultType)
    {
    }

    private BinaryOperator(Syntax.Kind syntaxKind, BinaryOperatorKind kind, Type lefType, Type rightType, Type resultType)
    {
        SyntaxKind = syntaxKind;
        Kind = kind;
        LeftType = lefType;
        RightType = rightType;
        ResultType = resultType;
    }

    public Syntax.Kind SyntaxKind { get; }
    public BinaryOperatorKind Kind { get; }
    public Type LeftType { get; }
    public Type RightType { get; }
    public Type ResultType { get; }

    private static readonly BinaryOperator[] _operators =
    {
        new BinaryOperator(Syntax.Kind.Plus, BinaryOperatorKind.Addition, typeof(int)),
        new BinaryOperator(Syntax.Kind.Minus, BinaryOperatorKind.Subtraction, typeof(int)),
        new BinaryOperator(Syntax.Kind.Asterisk, BinaryOperatorKind.Multiplication, typeof(int)),
        new BinaryOperator(Syntax.Kind.ForwardSlash, BinaryOperatorKind.Division, typeof(int)),

        new BinaryOperator(Syntax.Kind.EqualsEquals, BinaryOperatorKind.Equals, typeof(int), typeof(bool)),
        new BinaryOperator(Syntax.Kind.BangEquals, BinaryOperatorKind.NotEquals, typeof(int), typeof(bool)),

        new BinaryOperator(Syntax.Kind.AmpAmp, BinaryOperatorKind.LogicalAND, typeof(bool)),
        new BinaryOperator(Syntax.Kind.PipePipe, BinaryOperatorKind.LogicalOR, typeof(bool)),
        new BinaryOperator(Syntax.Kind.EqualsEquals, BinaryOperatorKind.Equals, typeof(bool)),
        new BinaryOperator(Syntax.Kind.BangEquals, BinaryOperatorKind.NotEquals, typeof(bool)),
    };

    public static BinaryOperator? Bind(Syntax.Kind kind, Type leftType, Type rightType)
    {
        foreach (var op in _operators)
        {
            if (op.SyntaxKind == kind && op.LeftType == leftType && op.RightType == rightType)
            {
                return op;
            }
        }

        return null;
    }
}