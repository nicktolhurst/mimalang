namespace Mima.CodeAnalysis.Binding;

internal sealed class UnaryExpression : Expression
{
    public UnaryExpression(UnaryOperator op, Expression operand)
    {
        Operator = op;
        Operand = operand;
    }

    public override Kind Kind => Kind.UnaryExpression;
    public override Type Type => Operator.ResultType;
    public UnaryOperator Operator { get; }
    public Expression Operand { get; }

}