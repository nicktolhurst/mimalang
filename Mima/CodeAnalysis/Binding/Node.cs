namespace Mima.CodeAnalysis.Binding;

internal abstract class Node
{
    public abstract BoundNodeKind Kind { get; }
}