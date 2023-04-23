namespace Mima.CodeAnalysis.Binding;

internal sealed class BoundBinaryExpression : BoundExpression
{
    public BoundBinaryExpression(BoundExpression left, BinaryOperator op, BoundExpression right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }

    public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
    public override Type Type => Operator.ResultType;

    public BoundExpression Left { get; }
    public BinaryOperator Operator { get; }
    public BoundExpression Right { get; }
}