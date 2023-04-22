namespace Mima.CodeAnalysis.Syntax;

public sealed class LiteralExpressionSyntax : ExpressionSyntax
{
    public LiteralExpressionSyntax(Token token) 
        : this(token, token.Value)
    {
    }

    public LiteralExpressionSyntax(Token token, object value)
    {
        Token = token;
        Value = value;
    }

    public override Kind Kind => Kind.LiteralExpression;
    public Token Token { get; }
    public object Value { get; }

    public override IEnumerable<Node> GetChildren()
    {
        yield return Token;
    }
}
