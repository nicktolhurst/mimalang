namespace Mima.CodeAnalysis.Syntax;

public sealed class SyntaxTree
{
    public SyntaxTree(IEnumerable<Diagnostic> diagnostics, ExpressionSyntax root, Token endOfFileToken)
    {
        Diagnostics = diagnostics.ToArray();
        Root = root;
        EndOfFileToken = endOfFileToken;
    }

    public static SyntaxTree Parse(string text)
    {
        var parser = new Parser(text);
        return parser.Parse();
    }

    public IReadOnlyList<Diagnostic> Diagnostics { get; }
    public ExpressionSyntax Root { get; }
    public Token EndOfFileToken { get; }
}