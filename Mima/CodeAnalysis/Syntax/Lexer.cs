using Mima.CodeAnalysis.Text;

namespace Mima.CodeAnalysis.Syntax;

internal sealed class Lexer
{
    private readonly DiagnosticBag _diagnostics = new();
    private readonly SourceText _text;

    private int _position;
    private int _start;

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

    private void Next()
    {
        _position++;
    }

    public Token Lex()
    {
        _start = _position;

        if (char.IsDigit(Current))
            return LexNumberToken();

        else if (char.IsWhiteSpace(Current))
            return LexWhiteSpaceToken();

        else if (char.IsLetter(Current))
            return LexIdentifierOrKeyword();

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

            _ => BadToken(Kind.BadToken, _text.ToString(_position, 1)),
        };
    }

    private Token LexIdentifierOrKeyword()
    {
        while (char.IsLetter(Current))
            Next();

        var length = _position - _start;
        var text = _text.ToString(_start, length);
        var kind = Facts.GetKeywordKind(text);
        var kindText = Facts.GetText(kind) ?? _text.ToString(_start, length);

        return new Token(kind, _start, kindText, null);
    }

    private Token LexWhiteSpaceToken()
    {
        while (char.IsWhiteSpace(Current))
            Next();
            
        var length = _position - _start;
        var kindText = Facts.GetText(Kind.Number)?? _text.ToString(_start, length);
        return new Token(Kind.WhiteSpace, _start, kindText, null);
    }

    private Token LexNumberToken()
    {
        while (char.IsDigit(Current))
            Next();

        var length = _position - _start;
        var text = _text.ToString(_start, length);
        if (!int.TryParse(text, out var value))
            _diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, typeof(int));

        var kindText = Facts.GetText(Kind.Number) ?? _text.ToString(_start, length);
        return new Token(Kind.Number, _start, kindText, value);
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