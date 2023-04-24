namespace Mima.CodeAnalysis.Syntax;

public sealed class ParenExpressionSyntax : ExpressionSyntax
{
    public ParenExpressionSyntax(Token openToken, ExpressionSyntax expression, Token closeToken)
    {
        OpenToken = openToken;
        Expression = expression;
        CloseToken = closeToken;
    }

    public override Kind Kind => Kind.ParenExpression;

    public Token OpenToken { get; }
    public ExpressionSyntax Expression { get; }
    public Token CloseToken { get; }
}