namespace Mima.CodeAnalysis.Syntax;

public sealed class BinaryExpressionSyntax : ExpressionSyntax
{
    public BinaryExpressionSyntax(ExpressionSyntax left, Token token, ExpressionSyntax right)
    {
        Left = left;
        Token = token;
        Right = right;
    }

    public override Kind Kind => Kind.BinaryExpression;
    public ExpressionSyntax Left { get; }
    public Token Token { get; }
    public ExpressionSyntax Right { get; }

    public override IEnumerable<Node> GetChildren()
    {
        yield return Left;
        yield return Token;
        yield return Right;
    }
}