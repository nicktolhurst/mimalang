namespace Mima.CodeAnalysis.Syntax;

public sealed class UnaryExpressionSyntax : ExpressionSyntax
{
    public UnaryExpressionSyntax(Token token, ExpressionSyntax operand)
    {
        Token = token;
        Operand = operand;
    }

    public override Kind Kind => Kind.UnaryExpression;
    public Token Token { get; }
    public ExpressionSyntax Operand { get; }
}