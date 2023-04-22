namespace Mima.CodeAnalysis.Binding;

internal sealed class LiteralExpression : Expression
{
    public LiteralExpression(object value)
    {
        Value = value;
    }

    public override Type Type => Value.GetType();

    public override Kind Kind => Kind.LiteralExpression;

    public object Value { get; }

}