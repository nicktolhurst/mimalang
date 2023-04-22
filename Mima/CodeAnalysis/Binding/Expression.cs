namespace Mima.CodeAnalysis.Binding;

internal abstract class Expression : Node
{
    public abstract Type Type { get; }
}