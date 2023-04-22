namespace Mima.CodeAnalysis.Binding;

internal sealed class BinaryExpression : Expression
{
    public BinaryExpression(Expression left, BinaryOperator op, Expression right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }

    public override Kind Kind => Kind.UnaryExpression;
    public override Type Type => Operator.ResultType;

    public Expression Left { get; }
    public BinaryOperator Operator { get; }
    public Expression Right { get; }
}