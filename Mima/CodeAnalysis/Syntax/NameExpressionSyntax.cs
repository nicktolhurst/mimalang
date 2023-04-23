namespace Mima.CodeAnalysis.Syntax;

public sealed class NameExpressionSyntax : ExpressionSyntax
{
    public NameExpressionSyntax(Token token)
    {
        Token = token;
    }

    public Token Token { get; }

    public override Kind Kind => Syntax.Kind.NameExpression;

    public override IEnumerable<Node> GetChildren()
    {
        yield return Token;
    }
}
