namespace Mima.CodeAnalysis.Syntax;
using System.Reflection;

public abstract class Node
{
    public abstract Kind Kind { get; }
    public IEnumerable<Node> GetChildren()
    {
        var props = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in props)
        {
            if (typeof(Node).IsAssignableFrom(prop.PropertyType))
            {
                var child = (Node?)prop.GetValue(this);
                
                if (child is not null)
                    yield return child;
            }
            else if (typeof(IEnumerable<Node>).IsAssignableFrom(prop.PropertyType))
            {
                var children = (IEnumerable<Node>?)prop.GetValue(this);

                if(children is not null)
                    foreach (var child in children)
                        yield return child;
            }
        }
    }
}