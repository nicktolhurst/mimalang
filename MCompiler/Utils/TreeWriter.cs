namespace Mima.MCompiler.Utils;

internal static class TreeWriter
{
    internal static void WriteTree(CodeAnalysis.Syntax.Node node, string indent = " ", bool isLast = true)
    {
        string marker = isLast ? "└───" : "├───";

        WriteColour(indent, ConsoleColor.DarkGray);
        WriteColour(marker, ConsoleColor.DarkGray);
        WriteColour(node.Kind, ConsoleColor.DarkYellow);

        if (node is CodeAnalysis.Syntax.Token token && token.Value != null)
        {
            Console.Write(" ");
            WriteColour(token.Value, ConsoleColor.Gray);
        }

        Console.WriteLine();

        indent += isLast ? "    " : "│   ";

        var lastChild = node.GetChildren().LastOrDefault();

        foreach (var child in node.GetChildren())
        {
            WriteTree(child, indent, child == lastChild);
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