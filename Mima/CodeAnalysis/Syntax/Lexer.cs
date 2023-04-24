using Mima.CodeAnalysis.Text;

namespace Mima.CodeAnalysis.Syntax;

internal sealed class Lexer
{
    private readonly string _text;
    private int _position;
    private readonly DiagnosticBag _diagnostics = new();

    public Lexer(string text)
    {
        _text = text;
    }

    public DiagnosticBag Diagnostics => _diagnostics;

    private char Current => Peek(0);

    private char LookAhead => Peek(1);

    private char Peek(int offset)
    {
        var index = _position + offset;

        if (index >= _text.Length)
            return '\0';

        return _text[index];
    }

    private void Next()
    {
        _position++;
    }

    public Token Lex()
    {
        var start = _position;

        if (char.IsDigit(Current))
            return LexNumberToken(start);

        else if (char.IsWhiteSpace(Current))
            return LexWhiteSpaceToken(start);

        else if (char.IsLetter(Current))
            return LexStringToken(start);

        return Current switch
        {
            '\0' => new Token(Kind.EOF, _position, "\0", null),
            '!' when LookAhead == '=' => LexToken(Kind.BangEquals),
            '!' => LexToken(Kind.Bang),
            '=' when LookAhead == '=' => LexToken(Kind.EqualsEquals),
            '=' => LexToken(Kind.Equals),
            '+' => LexToken(Kind.Plus),
            '-' => LexToken(Kind.Minus),
            '/' => LexToken(Kind.ForwardSlash),
            '*' => LexToken(Kind.Asterisk),
            '&' when LookAhead == '&' => LexToken(Kind.AmpAmp),
            '|' when LookAhead == '|' => LexToken(Kind.PipePipe),
            '<' when LookAhead == '|' => LexToken(Kind.Equals, "<|"),
            '(' => LexToken(Kind.OpenParen),
            ')' => LexToken(Kind.CloseParen),

            _ => BadToken(Kind.BadToken, _text.Substring(_position - 1, 1)),
        };
    }

    private Token LexStringToken(int start)
    {
        while (char.IsLetter(Current))
            Next();

        var text = _text[start.._position];

        var kind = Facts.GetKeywordKind(text);

        return new Token(kind, start, text, null);
    }

    private Token LexWhiteSpaceToken(int start)
    {
        while (char.IsWhiteSpace(Current))
            Next();

        var length = _position - start;
        var text = _text.Substring(start, length);

        return new Token(Kind.WhiteSpace, start, text, null);
    }

    private Token LexNumberToken(int start)
    {
        while (char.IsDigit(Current))
            Next();

        var text = _text[start.._position];

        if (!int.TryParse(text, out var value))
            _diagnostics.ReportInvalidNumber(new TextSpan(start, _position - start), _text, typeof(int));

        return new Token(Kind.Number, start, text, value);
    }

    private Token LexToken(Kind kind)
    {
        var position = _position;
        var text = Facts.GetText(kind) ?? string.Empty;

        _position += text.Length;
        return new Token(kind, position, text, null);
    }

    private Token LexToken(Kind kind, string text)
    {
        var position = _position;

        _position += text.Length;
        return new Token(kind, position, text, null);
    }

    private Token LexToken(Kind kind, int position, string text, object? value)
    {
        _position += text.Length;
        return new Token(kind, position, text, value);
    }

    private Token BadToken(Kind kind, string text)
    {
        _diagnostics.ReportBadCharacter(_position, Current);

        return LexToken(kind, _position, text, null);
    }
}