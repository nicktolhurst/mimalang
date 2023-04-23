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
            '+' => LexToken(Kind.Plus, _position, "+", null),
            '-' => LexToken(Kind.Minus, _position, "-", null),
            '/' => LexToken(Kind.ForwardSlash, _position, "/", null),
            '*' => LexToken(Kind.Asterisk, _position, "*", null),

            // Equivalence operators:
            '=' when LookAhead == '=' => LexToken(Kind.EqualsEquals, _position, "==", null),
            '!' when LookAhead == '=' => LexToken(Kind.BangEquals, _position, "!=", null),

            // Scope operators:
            '(' => LexToken(Kind.OpenParen, _position, "(", null),
            ')' => LexToken(Kind.CloseParen, _position, ")", null),

            // Logic operators:
            '&' when LookAhead == '&' => LexToken(Kind.AmpAmp, _position, "&&", null),
            '|' when LookAhead == '|' => LexToken(Kind.PipePipe, _position, "||", null),

            // Negation operators:
            '!' => LexToken(Kind.Bang, _position, "!", null),

            // Assignment operators:
            '=' => LexToken(Kind.Equals, _position, "=", null),
            '<' when LookAhead == '|' => LexToken(Kind.Equals, _position, "<|", null),

            _ => BadToken(Kind.BadToken, _position, _text.Substring(_position - 1, 1), null),
        };
    }

    private Token LexToken(Kind kind, int position, string text, object? value)
    {
        _position += text.Length;
        return new Token(kind, position, text, value);
    }

    private Token BadToken(Kind kind, int position, string text, object? value)
    {
        _diagnostics.ReportBadCharacter(position, Current);

        return LexToken(kind, position, text, value);
    }
}