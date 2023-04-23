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

        if (_position >= _text.Length)
            return new Token(Kind.EOF, _position, "\0", null);

        if (char.IsDigit(Current))
        {
            while (char.IsDigit(Current))
                Next();

            var length = _position - start;
            var text = _text.Substring(start, length);

            if (!int.TryParse(text, out var value))
            {
                _diagnostics.ReportInvalidNumber(new TextSpan(start, length), _text, typeof(int));
            }

            return new Token(Kind.Number, start, text, value);
        }

        if (char.IsWhiteSpace(Current))
        {
            while (char.IsWhiteSpace(Current))
                Next();

            var length = _position - start;
            var text = _text.Substring(start, length);

            return new Token(Kind.WhiteSpace, start, text, null);
        }

        if (char.IsLetter(Current))
        {
            while (char.IsLetter(Current))
                Next();

            var length = _position - start;
            var text = _text.Substring(start, length);

            var kind = Facts.GetKeywordKind(text);

            return new Token(kind, start, text, null);
        }

        return Current switch
        {
            // Arithmetic operators:
            '+' => LexToken(Kind.Plus),
            '-' => LexToken(Kind.Minus),
            '/' => LexToken(Kind.ForwardSlash),
            '*' => LexToken(Kind.Asterisk),

            // Equivalence operators:
            '=' when LookAhead == '=' => LexToken(Kind.EqualsEquals),
            '!' when LookAhead == '=' => LexToken(Kind.BangEquals),

            // Scope operators:
            '(' => LexToken(Kind.OpenParen),
            ')' => LexToken(Kind.CloseParen),

            // Logic operators:
            '&' when LookAhead == '&' => LexToken(Kind.AmpAmp),
            '|' when LookAhead == '|' => LexToken(Kind.PipePipe),

            // Negation operators:
            '!' => LexToken(Kind.Bang),

            // Assignment operators:
            '=' => LexToken(Kind.Equals),
            '<' when LookAhead == '|' => LexToken(Kind.Equals, "<|"),

            _ => BadToken(Kind.BadToken, _text.Substring(_position - 1, 1)),
        };
    }

    private Token LexToken(Kind kind)
    {
        var position = _position;
        var text = Facts.GetText(kind);

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