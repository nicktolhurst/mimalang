namespace Mima.CodeAnalysis.Syntax;

public abstract class Node
{
    public abstract Kind Kind { get; }
    public abstract IEnumerable<Node> GetChildren();
}