namespace Mima.CodeAnalysis.Binding;

internal abstract class BoundExpression : Node
{
    public abstract Type Type { get; }
}