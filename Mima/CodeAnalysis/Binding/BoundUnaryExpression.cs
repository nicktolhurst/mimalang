namespace Mima.CodeAnalysis.Binding;

internal sealed class BoundUnaryExpression : BoundExpression
{
    public BoundUnaryExpression(UnaryOperator op, BoundExpression operand)
    {
        Operator = op;
        Operand = operand;
    }

    public override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;
    public override Type Type => Operator.ResultType;
    public UnaryOperator Operator { get; }
    public BoundExpression Operand { get; }

}