using Mima.CodeAnalysis;
using Mima.CodeAnalysis.Syntax;
using Mima.MCompiler.Utils;

bool showTree = false;
var variables = new Dictionary<VariableSymbol, object>();

while (true)
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;

    Console.Write("🐶 ");
    var line = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(line)) return;

    switch (line)
    {
        case "#showTree":
            showTree = !showTree;
            Console.WriteLine(showTree ? "Showing syntax tree." : "Not showing syntax tree.");
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
        TreeWriter.WriteTree(syntaxTree.Root);
    }

    if (!result.Diagnostics.Any())
    {
        Console.WriteLine(result.Value);
    }
    else
    {
        foreach (var diagnostic in result.Diagnostics)
        {
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkRed;
            Console.WriteLine(diagnostic);
            Console.ResetColor();

            var prefix = line[..diagnostic.Span.Start];
            var error = line.Substring(diagnostic.Span.Start, diagnostic.Span.Length);
            var suffix = line[diagnostic.Span.End..];

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
}
