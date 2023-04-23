using Mima.CodeAnalysis.Syntax;

namespace Mima.CodeAnalysis.Syntax;

public class LexerTests
{
    [Theory]
    [MemberData(nameof(GetTokensData))]
    public void Lexer_Lexes_Token(Kind kind, string text)
    {
        var tokens = SyntaxTree.ParseTokens(text);
        var token = Assert.Single(tokens);

        Assert.Equal(kind, token.Kind);
        Assert.Equal(text, token.Text);
    }

    [Theory]
    [MemberData(nameof(GetTokenPairsData))]
    public void Lexer_Lexes_TokenPairs(Kind t1Kind, string t1Text,
                                    Kind t2Kind, string t2Text)
    {
        var text = t1Text + t2Text;

        var tokens = SyntaxTree.ParseTokens(text).ToArray();

        Assert.Equal(2, tokens.Length);
        Assert.Equal(tokens[0].Kind, t1Kind);
        Assert.Equal(tokens[0].Text, t1Text);
        Assert.Equal(tokens[1].Kind, t2Kind);
        Assert.Equal(tokens[1].Text, t2Text);
    }


    [Theory]
    [MemberData(nameof(GetTokenPairsWithSeparatorData))]
    public void Lexer_Lexes_TokenPairsWithSeparator(Kind t1Kind, string t1Text,
                                        Kind sKind, string sText,
                                        Kind t2Kind, string t2Text)
    {
        var text = t1Text + sText + t2Text;

        var tokens = SyntaxTree.ParseTokens(text).ToArray();

        Assert.Equal(3, tokens.Length);
        Assert.Equal(tokens[0].Kind, t1Kind);
        Assert.Equal(tokens[0].Text, t1Text);
        Assert.Equal(tokens[1].Kind, sKind);
        Assert.Equal(tokens[1].Text, sText);
        Assert.Equal(tokens[2].Kind, t2Kind);
        Assert.Equal(tokens[2].Text, t2Text);
    }

    public static IEnumerable<object[]> GetTokensData()
    {
        foreach (var (kind, text) in GetTokens().Concat(GetSeparators()))
            yield return new object[] { kind, text };
    }

    public static IEnumerable<object[]> GetTokenPairsData()
    {
        foreach (var (t1Kind, t1Text, t2Kind, t2Text) in GetTokenPairs())
            yield return new object[] { t1Kind, t1Text, t2Kind, t2Text };
    }

    public static IEnumerable<object[]> GetTokenPairsWithSeparatorData()
    {
        foreach (var (t1Kind, t1Text, sKind, sText, t2Kind, t2Text) in GetTokenPairsWithSeparator())
            yield return new object[] { t1Kind, t1Text, sKind, sText, t2Kind, t2Text };
    }

    private static IEnumerable<(Kind kind, string text)> GetTokens()
    {
        return new [] {
            (Kind.AmpAmp, "&&"),
            (Kind.PipePipe, "||"),

            (Kind.Equals, "="),
            (Kind.Bang, "!"),
            (Kind.EqualsEquals, "=="),
            (Kind.BangEquals, "!="),

            (Kind.Identifier, "a"),
            (Kind.Identifier, "abc"),
            (Kind.Number, "1"),
            (Kind.Number, "123"),

            (Kind.Asterisk, "*"),
            (Kind.ForwardSlash, "/"),
            (Kind.Plus, "+"),
            (Kind.Minus, "-"),

            (Kind.CloseParen, ")"),
            (Kind.OpenParen, "("),

            (Kind.False, "false"),
            (Kind.True, "true"),
        };
    }

    private static IEnumerable<(Kind kind, string text)> GetSeparators()
    {
        return new[] {
            (Kind.WhiteSpace, " "),
            (Kind.WhiteSpace, "  "),
            (Kind.WhiteSpace, "\r"),
            (Kind.WhiteSpace, "\n"),
            (Kind.WhiteSpace, "\r\n"),
        };
    }

    private static bool RequiresSeparator(Kind t1Kind, Kind t2Kind)
    {
        var t1IsKeyword = t1Kind is Kind.True or 
                                    Kind.False;

        var t2IsKeyword = t2Kind is Kind.True or
                                    Kind.False;

        if(t1IsKeyword && t2IsKeyword)
            return true;

        if(t1IsKeyword && t2Kind == Kind.Identifier)
            return true;

        if(t1Kind == Kind.Identifier && t2IsKeyword)
            return true;

        if(t1Kind == Kind.Identifier && t2Kind == Kind.Identifier)
            return true;

        if (t1Kind == Kind.Number && t2Kind == Kind.Number)
            return true;

        if (t1Kind == Kind.Bang && t2Kind == Kind.Equals)
            return true;

        if (t1Kind == Kind.Bang && t2Kind == Kind.EqualsEquals)
            return true;

        if (t1Kind == Kind.Equals && t2Kind == Kind.Equals)
            return true;

        if (t1Kind == Kind.Equals && t2Kind == Kind.EqualsEquals)
            return true;

        return false;
    }

    private static IEnumerable<(Kind t1Kind, string t1Text, Kind t2Kind, string t2Text)> GetTokenPairs()
    {
        foreach (var (_t1Kind, _t1Text) in GetTokens())
        {
            foreach (var (_t2Kind, _t2Text) in GetTokens())
            {
                if (!RequiresSeparator(_t1Kind, _t2Kind))
                    yield return (_t1Kind, _t1Text, _t2Kind, _t2Text);
            }
        }
    }

    private static IEnumerable<(Kind t1Kind, string t1Text, Kind sKind, string sText, Kind t2Kind, string t2Text)> GetTokenPairsWithSeparator()
    {
        foreach (var (_t1Kind, _t1Text) in GetTokens())
        {
            foreach (var (_t2Kind, _t2Text) in GetTokens())
            {
                if (RequiresSeparator(_t1Kind, _t2Kind))
                {
                    foreach (var (_sKind, _sText) in GetSeparators())
                        yield return (_t1Kind, _t1Text, _sKind, _sText, _t2Kind, _t2Text);
                }
            }
        }
    }
}