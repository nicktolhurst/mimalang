using Mima.CodeAnalysis;
using Mima.CodeAnalysis.Syntax;

bool showTree = false;
bool showCompactTree = false;
var variables = new Dictionary<VariableSymbol, object?>();

while (true)
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;

    Console.Write("🐶 ");
    var line = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(line)) return;

    switch (line)
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

    var syntaxTree = SyntaxTree.Parse(line);
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
        var text = syntaxTree.Text;
        foreach (var diagnostic in result.Diagnostics)
        {
            var lineIndex = text.GetLineIndex(diagnostic.Span.Start);
            var lineNumber = lineIndex + 1;
            var character = diagnostic.Span.Start - text.Lines[lineIndex].Start + 1;

            Console.WriteLine();
            WriteDiagnosticEntry("Source", "Console Input");
            WriteDiagnosticEntry("Position", $"Line: {lineNumber}, Char: {character}.");
            WriteDiagnosticEntry("Type", diagnostic.DiagnosticType.ToString());
            WriteDiagnosticEntry("Error", diagnostic.Message);
            WriteDiagnosticErrorEntry("Details", line, diagnostic);
            Console.WriteLine();
        }

        Console.WriteLine();
    }
}

static void WriteDiagnosticEntry(string heading, string value, bool asError = false, string separator = " ┃ ", int col1Width = 10)
{
    for (int i = 0; i < (col1Width - heading.Length); i++)
        Console.Write(" ");

    Console.ForegroundColor = ConsoleColor.DarkCyan;
    Console.Write(heading);
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write(separator);
    if(asError)
        Console.ForegroundColor = ConsoleColor.DarkRed;
    else
        Console.ResetColor();
    Console.Write(value);
    Console.WriteLine();
}

static void WriteDiagnosticErrorEntry(string heading, string line, Diagnostic diagnostic, string separator = " ┃ ", int col1Width = 10)
{
    for (int i = 0; i < (col1Width - heading.Length); i++)
        Console.Write(" ");

    var prefix = line[..diagnostic.Span.Start];
    var error = line.Substring(diagnostic.Span.Start, diagnostic.Span.Length);
    var suffix = line[diagnostic.Span.End..];

    Console.ForegroundColor = ConsoleColor.DarkCyan;
    Console.Write(heading);
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.Write(separator);
    Console.ResetColor();
    Console.Write(prefix);
    Console.ForegroundColor = ConsoleColor.DarkRed;
    Console.Write(error);
    Console.ResetColor();
    Console.Write(suffix);
    Console.WriteLine();
}