namespace Mima.CodeAnalysis.Syntax;
using System.Collections.Immutable;

public sealed class SyntaxTree
{
    public SyntaxTree(ImmutableArray<Diagnostic> diagnostics, ExpressionSyntax root, Token endOfFileToken)
    {
        Diagnostics = diagnostics;
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

    public ImmutableArray<Diagnostic> Diagnostics { get; }
    public ExpressionSyntax Root { get; }
    public Token EndOfFileToken { get; }
}