namespace Mima.CodeAnalysis.Syntax;

public sealed class AssignmentExpressionSyntax : ExpressionSyntax
{
    public AssignmentExpressionSyntax(Token token, Token equalsToken, ExpressionSyntax expression)
    {
        Token = token;
        EqualsToken = equalsToken;
        Expression = expression;
    }

    public Token Token { get; }
    public Token EqualsToken { get; }
    public ExpressionSyntax Expression { get; }

    public override Kind Kind => Syntax.Kind.AssignmentExpression;
}