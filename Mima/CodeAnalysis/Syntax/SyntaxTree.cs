namespace Mima.CodeAnalysis.Syntax;
using System.Collections.Immutable;
using Mima.CodeAnalysis.Text;

public sealed class SyntaxTree
{
    public SyntaxTree(SourceText text, ImmutableArray<Diagnostic> diagnostics, ExpressionSyntax root, Token endOfFileToken)
    {
        Text = text;
        Diagnostics = diagnostics;
        Root = root;
        EndOfFileToken = endOfFileToken;
    }

    public SourceText Text { get; }
    public ImmutableArray<Diagnostic> Diagnostics { get; }
    public ExpressionSyntax Root { get; }
    public Token EndOfFileToken { get; }

    public static SyntaxTree Parse(string text)
    {
        var sourceText = SourceText.From(text);
        return Parse(sourceText);
    }

    public static SyntaxTree Parse(SourceText text)
    {
        var parser = new Parser(text);
        return parser.Parse();
    }

    public static IEnumerable<Token> ParseTokens(string text)
    {
        var sourceText = SourceText.From(text);
        return ParseTokens(sourceText);
    }
    
    public static IEnumerable<Token> ParseTokens(SourceText text)
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
}