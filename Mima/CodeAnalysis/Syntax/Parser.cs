namespace Mima.CodeAnalysis.Syntax;

internal sealed class Parser
{
    private readonly Token[] _tokens;
    private int _position;
    private readonly DiagnosticBag _diagnostics = new();

    public Parser(string text)
    {
        var tokens = new List<Token>();

        var lexer = new Lexer(text);

        Token token;

        do
        {
            token = lexer.Lex();

            if (token.Kind != Kind.WhiteSpace &&
                token.Kind != Kind.BadToken)
            {
                tokens.Add(token);
            }

        } while (token.Kind != Kind.EOF);

        _tokens = tokens.ToArray();
        _diagnostics.AddRange(lexer.Diagnostics);
    }

    public DiagnosticBag Diagnostics => _diagnostics;

    public SyntaxTree Parse()
    {
        var expression = ParseExpression();

        var eof = MatchToken(Kind.EOF);

        return new SyntaxTree(_diagnostics, expression, eof);
    }

    private ExpressionSyntax ParseExpression()
    {
        return ParseAssignmentExpression();
    }

    private ExpressionSyntax ParseAssignmentExpression()
    {
        if(Peek(0).Kind == Kind.Identifier &&
           Peek(1).Kind == Kind.Equals)
        {
            var leftToken = NextToken();
            var operatorToken = NextToken();
            var rightExpression = ParseAssignmentExpression();
            return new AssignmentExpressionSyntax(leftToken, operatorToken, rightExpression);
        }

        return ParseBinaryExpression();
    }

    private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
    {
        ExpressionSyntax left;

        var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();

        if(unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
        {
            var operatorToken = NextToken();
            var operand = ParseBinaryExpression(unaryOperatorPrecedence);
            left = new UnaryExpressionSyntax(operatorToken, operand);
        }
        else
        {
            left = ParsePrimaryExpression();
        }

        while(true) 
        {
            var precedence = Current.Kind.GetBinaryOperatorPrecedence();
            if (precedence == 0 || precedence <= parentPrecedence)
                break;

            var operatorToken = NextToken();
            var right = ParseBinaryExpression(precedence);
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParsePrimaryExpression()
    {
        switch (Current.Kind)
        {
            case Kind.OpenParen:
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = MatchToken(Kind.CloseParen);
                return new ParenExpressionSyntax(left, expression, right);
            }

            case Kind.True:
            case Kind.False:
            {
                var keywordToken = NextToken();
                var value = keywordToken.Kind == Kind.True;
                return new LiteralExpressionSyntax(keywordToken, value);
            }

            case Kind.Identifier:
            {
                var identifierToken = NextToken();
                return new NameExpressionSyntax(identifierToken);
            }

            default:
            {
                var token = MatchToken(Kind.Number);
                return new LiteralExpressionSyntax(token);
            }
        }
    }

    private Token Peek(int offset)
    {
        var index = _position + offset;

        if (index >= _tokens.Length)
            return _tokens[^1];

        return _tokens[index];
    }

    private Token Current => Peek(0);

    private Token NextToken()
    {
        var current = Current;
        _position++;
        return current;
    }

    private Token MatchToken(Kind kind)
    {
        if (Current.Kind == kind)
        {
            return NextToken();
        }

        _diagnostics.ReportUnexpectedToken(Current.TextSpan, Current.Kind, kind);

        return new Token(kind, Current.Position, string.Empty, null);
    }
}