using Mima.CodeAnalysis.Text;

namespace Mima.CodeAnalysis.Syntax;

internal sealed class Lexer
{
    private readonly DiagnosticBag _diagnostics = new();
    private readonly SourceText _text;

    private int _position;

    private int _start;
    private Kind _kind;
    private object? _value;

    public Lexer(SourceText text)
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

    public Token Lex()
    {
        // Reset values.
        (_start, _kind, _value) = (_position, Kind.BadToken, null);

        (_start, _kind, _value) = Current switch 
        {
            '0' or
            '1' or
            '2' or
            '3' or
            '4' or
            '5' or
            '6' or
            '7' or
            '8' or
            '9' => LexNumberToken(),

            ' ' or
            '\r' or
            '\n' or
            '\t' => LexWhiteSpaceToken(),

            '\0' => LexToken(Kind.EOF),

            '!' when LookAhead == '=' => LexToken(Kind.BangEquals, "!="),
            '!' => LexToken(Kind.Bang, "!"),

            '=' when LookAhead == '=' => LexToken(Kind.EqualsEquals, "=="),
            '=' => LexToken(Kind.Equals, "="),

            '&' when LookAhead == '&' => LexToken(Kind.AmpAmp, "&&"),
            '|' when LookAhead == '|' => LexToken(Kind.PipePipe, "||"),
            '<' when LookAhead == '|' => LexToken(Kind.Equals, "<|"),
            '+' => LexToken(Kind.Plus, "+"),
            '-' => LexToken(Kind.Minus, "-"),
            '/' => LexToken(Kind.ForwardSlash, "/"),
            '*' => LexToken(Kind.Asterisk, "*"),
            '(' => LexToken(Kind.OpenParen, "("),
            ')' => LexToken(Kind.CloseParen, ")"),
            _   => LexDefaultToken()

        };

        var length = _position - _start;
        var text = Facts.GetText(_kind);
        text ??= _text.ToString(_start, length);

        return new Token(_kind, _start, text, _value);
    }

    private (int, Kind, object?) LexDefaultToken() => 
        char.IsLetter(Current) 
            ? LexIdentifierOrKeyword()
            : char.IsWhiteSpace(Current) 
                ? LexWhiteSpaceToken() 
                : LexBadToken();

    private (int, Kind, object?) LexWhiteSpaceToken()
    {
        while (char.IsWhiteSpace(Current))
            _position++;

        return (_start, Kind.WhiteSpace, _value);
    }

    private (int, Kind, object?) LexNumberToken()
    {
        while (char.IsDigit(Current))
            _position++;

        var length = _position - _start;
        var text = _text.ToString(_start, length);
        if (!int.TryParse(text, out var value))
            _diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, typeof(int));

        return (_start, Kind.Number, value);
    }

    private (int, Kind, object?) LexIdentifierOrKeyword()
    {
        while (char.IsLetter(Current))
            _position++;

        var length = _position - _start;
        var text = _text.ToString(_start, length);
        var kind = Facts.GetKeywordKind(text);

        return (_start, kind, _value);
    }

    private (int, Kind, object?) LexToken(Kind kind) => (_start, kind, _value);

    private (int, Kind, object?) LexToken(Kind kind, string text)
    {
        _position += text.Length;
        return (_start, kind, _value);
    }

    private (int, Kind, object?) LexBadToken()
    {
        _diagnostics.ReportBadCharacter(_position, Current);
        _position++;
        return(_start, _kind, _value);
    }
}