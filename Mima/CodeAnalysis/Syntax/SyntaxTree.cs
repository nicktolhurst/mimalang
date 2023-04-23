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

    public static IEnumerable<Token> ParseTokens(string text)
    {
        var lexer = new Lexer(text);

        while(true)
        {
            var token = lexer.Lex();

            if (token.Kind == Kind.EOF)
                break;
            
            yield return token;
        }
    }

    public IReadOnlyList<Diagnostic> Diagnostics { get; }
    public ExpressionSyntax Root { get; }
    public Token EndOfFileToken { get; }
}