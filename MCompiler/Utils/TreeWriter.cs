namespace Mima.MCompiler.Utils;

internal static class TreeWriter
{
    internal static void WriteTree(CodeAnalysis.Syntax.Node node, string indent = " ", bool isLast = true)
    {
        string marker = isLast ? "└───" : "├───";

        WriteColour(indent, ConsoleColor.DarkGray);
        WriteColour(marker, ConsoleColor.DarkGray);
        WriteColour(node.Kind, ConsoleColor.DarkYellow);

        if (node is CodeAnalysis.Syntax.Token nonNullToken && nonNullToken.Value != null)
        {
            Console.Write(" ");
            WriteColour(nonNullToken.Value, ConsoleColor.DarkGreen);
        }

        if (node is CodeAnalysis.Syntax.Token identifierToken && identifierToken.Kind == CodeAnalysis.Syntax.Kind.Identifier)
        {
            Console.Write(" ");
            WriteColour(identifierToken.Text, ConsoleColor.DarkBlue);
        }


        Console.WriteLine();

        indent += isLast ? "    " : "│   ";

        var lastChild = node.GetChildren().LastOrDefault();

        foreach (var child in node.GetChildren())
        {
            WriteTree(child, indent, child == lastChild);
        }
    }

    internal static void WriteCompactTree(CodeAnalysis.Syntax.Node node, string indent = " ", bool isLast = true, bool isFirst = false)
    {
        if (node is CodeAnalysis.Syntax.Token token)
        {
            WriteColour(indent, ConsoleColor.DarkGray);

            if (token.Value != null)
            {
                Console.Write(" ");
                WriteColour(token.Value, ConsoleColor.DarkGreen);
            }
            else
            {
                Console.Write(" ");
                WriteColour(token.Text, ConsoleColor.DarkBlue);
            }

            Console.WriteLine();
        }

        indent += "    ";

        var lastChild = node.GetChildren().LastOrDefault();
        var firstChild = node.GetChildren().FirstOrDefault();

        foreach (var child in node.GetChildren())
        {
            WriteCompactTree(child, indent, child == lastChild, child == firstChild);
        }
    }

    private static void WriteColour(object obj, ConsoleColor colour)
    {
        var originalColour = Console.ForegroundColor;
        Console.ForegroundColor = colour;
        Console.Write(obj);
        Console.ForegroundColor = originalColour;
    }
}