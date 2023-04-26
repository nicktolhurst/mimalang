using System.Text;
using Mima.CodeAnalysis;
using Mima.CodeAnalysis.Syntax;
using Mima.CodeAnalysis.Text;

bool showTree = false;
bool showCompactTree = false;
var variables = new Dictionary<VariableSymbol, object?>();
var textBuilder = new StringBuilder();

while (true)
{
    Console.OutputEncoding = Encoding.UTF8;

    if(textBuilder.Length == 0)
        Console.Write("🐶 ");
    else
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.Write(" │ ");
        Console.ResetColor();
    }

    var input = Console.ReadLine();

    var isBlank = string.IsNullOrWhiteSpace(input);

    if(textBuilder.Length == 0)
    {
        if(isBlank)
            break;
        
        switch (input)
        {
            case "#showTree":
            case "#st":
                showTree = !showTree;
                Console.WriteLine(showTree ? "Showing syntax tree." : "Not showing syntax tree.");
                continue;
            case "#showCompactTree":
            case "#sct":
                showCompactTree = !showCompactTree;
                Console.WriteLine(showCompactTree ? "Showing compact tree." : "Not showing compact tree.");
                continue;
            case "#cls":
            case "#clear":
                Console.Clear();
                continue;
            case "#compile":
                Console.Clear();
                continue;
        }
    }

    textBuilder.AppendLine(input);
    var text = textBuilder.ToString();

    var syntaxTree = SyntaxTree.Parse(text);

    if (!isBlank && syntaxTree.Diagnostics.Any())
        continue;

    var compilation = new Compilation(syntaxTree);
    var result = compilation.Evaluate(variables);

    if (showTree)
    {
        syntaxTree.Root.WriteTo(Console.Out);
    }
    if (!result.Diagnostics.Any())
    {
        Console.WriteLine(result.Value);
    }
    else
    {
        foreach (var diagnostic in result.Diagnostics)
        {
            var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
            var line = syntaxTree.Text.Lines[lineIndex];
            var lineNumber = lineIndex + 1;
            var character = diagnostic.Span.Start - line.Start + 1;

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write($"[{lineNumber} {character}]: ");
            Console.WriteLine(diagnostic);
            Console.ResetColor();

            var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
            var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

            var prefix = syntaxTree.Text.ToString(prefixSpan);
            var error = syntaxTree.Text.ToString(diagnostic.Span);
            var suffix = syntaxTree.Text.ToString(suffixSpan);

            Console.Write("    ");
            Console.Write(prefix);

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.Write(error);
            Console.ResetColor();

            Console.Write(suffix);

            Console.WriteLine();
        }

        Console.WriteLine();
    }

    textBuilder.Clear();
}

// static void WriteDiagnosticEntry(string heading, string value, bool asError = false, string separator = " ┃ ", int col1Width = 10)
// {
//     for (int i = 0; i < (col1Width - heading.Length); i++)
//         Console.Write(" ");

//     Console.ForegroundColor = ConsoleColor.DarkCyan;
//     Console.Write(heading);
//     Console.ForegroundColor = ConsoleColor.DarkGray;
//     Console.Write(separator);
//     if(asError)
//         Console.ForegroundColor = ConsoleColor.DarkRed;
//     else
//         Console.ResetColor();
//     Console.Write(value);
//     Console.WriteLine();
// }

// static void WriteDiagnosticErrorEntry(string heading, string line, Diagnostic diagnostic, string separator = " ┃ ", int col1Width = 10)
// {
//     for (int i = 0; i < (col1Width - heading.Length); i++)
//         Console.Write(" ");

//     var prefix = [..diagnostic.Span.Start];
//     var error = line.Substring(diagnostic.Span.Start, diagnostic.Span.Length);
//     var suffix = line[diagnostic.Span.End..];

//     Console.ForegroundColor = ConsoleColor.DarkCyan;
//     Console.Write(heading);
//     Console.ForegroundColor = ConsoleColor.DarkGray;
//     Console.Write(separator);
//     Console.ResetColor();
//     Console.Write(prefix);
//     Console.ForegroundColor = ConsoleColor.DarkRed;
//     Console.Write(error);
//     Console.ResetColor();
//     Console.Write(suffix);
//     Console.WriteLine();
// }