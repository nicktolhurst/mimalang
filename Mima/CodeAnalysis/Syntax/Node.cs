namespace Mima.CodeAnalysis.Syntax;

using System;
using System.Reflection;
using Mima.CodeAnalysis.Text;

public abstract class Node
{
    public abstract Kind Kind { get; }
    public virtual TextSpan Span
    {
        get 
        {
            var first = GetChildren().First().Span;
            var last = GetChildren().Last().Span;
            return TextSpan.FromBounds(first.Start, last.End);
        }
    }

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

    public void WriteTo(TextWriter writer)
    {
        WriteTree(writer, this);
    }

    private static void WriteTree(TextWriter writer, Node node, string indent = " ", bool isLast = true)
    {
        string marker = isLast ? "└───" : "├───";


        Console.ForegroundColor = ConsoleColor.DarkGray;
        writer.Write(indent);
        writer.Write(marker);

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        writer.Write(node.Kind);

        if (node is Token t && t.Value != null)
        {
            writer.Write(" ");
            writer.Write(t.Value);
        }

        if (node is Token idt && idt.Kind == Kind.Identifier)
        {
            writer.Write(" ");
            writer.Write(idt.Text, ConsoleColor.DarkBlue);
        }

        writer.WriteLine();

        indent += isLast ? "    " : "│   ";

        var lastChild = node.GetChildren().LastOrDefault();

        foreach (var child in node.GetChildren())
            WriteTree(writer, child, indent, child == lastChild);

        Console.ResetColor();
    }

    public override string ToString()
    {
        using var writer = new StringWriter();

        WriteTo(writer);

        return writer.ToString();
    }
}