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

        switch (Current)
        {
            case '+':
                return new Token(Kind.Plus, _position++, "+", null);
            case '-':
                return new Token(Kind.Minus, _position++, "-", null);
            case '/':
                return new Token(Kind.ForwardSlash, _position++, "/", null);
            case '*':
                return new Token(Kind.Asterisk, _position++, "*", null);
            case '(':
                return new Token(Kind.OpenParen, _position++, "(", null);
            case ')':
                return new Token(Kind.CloseParen, _position++, ")", null);
            case '&':
                if (LookAhead == '&')
                {
                    _position += 2;
                    return new Token(Kind.AmpAmp, start, "&&", null);
                }
                break;
            case '|':
                if (LookAhead == '|')
                {
                    _position += 2;
                    return new Token(Kind.PipePipe, start, "||", null);
                }
                break;
            case '=':
                if (LookAhead == '=')
                {
                    _position += 2;
                    return new Token(Kind.EqualsEquals, start, "==", null);
                }
                else
                {
                    _position += 1;
                    return new Token(Kind.Equals, start, "=", null);
                }
            case '!':
                if (LookAhead == '=')
                {
                    _position += 2;
                    return new Token(Kind.BangEquals, start, "==", null);
                }
                else
                {
                    _position += 1;
                    return new Token(Kind.Bang, start, "!", null);
                }
        }

        _diagnostics.ReportBadCharacter(_position, Current);

        return new Token(Kind.BadToken, _position++, _text.Substring(_position - 1, 1), null);
    }
}