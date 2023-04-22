namespace Mima.CodeAnalysis.Binding;


internal sealed class Binder 
{
    private readonly DiagnosticBag _diagnostics = new();

    public DiagnosticBag Diagnostics => _diagnostics;

    public Expression BindExpression(Syntax.ExpressionSyntax syntax)
    {
        return syntax.Kind switch
        {
            Syntax.Kind.LiteralExpression => BindLiteralExpression((Syntax.LiteralExpressionSyntax)syntax),
            Syntax.Kind.UnaryExpression => BindUnaryExpression((Syntax.UnaryExpressionSyntax)syntax),
            Syntax.Kind.BinaryExpression => BindBinaryExpression((Syntax.BinaryExpressionSyntax)syntax),
            Syntax.Kind.ParenExpression => BindExpression(((Syntax.ParenExpressionSyntax)syntax).Expression),
            _ => throw new Exception($"Unexpected syntax {syntax.Kind}"),
        };
    }

    private static Expression BindLiteralExpression(Syntax.LiteralExpressionSyntax syntax)
    {

        var value = syntax.Value ?? 0;
        return new LiteralExpression(value);
    }

    private Expression BindUnaryExpression(Syntax.UnaryExpressionSyntax syntax)
    {
        var boundOperand = BindExpression(syntax.Operand);
        var boundOperator = UnaryOperator.Bind(syntax.Token.Kind, boundOperand.Type);
        
        if(boundOperator == null)
        {
            _diagnostics.ReportUndefinedUnaryOperator(syntax.Token.TextSpan, syntax.Token.Text,  boundOperand.Type);
            return boundOperand;
        }

        return new UnaryExpression(boundOperator, boundOperand);
    }

    private Expression BindBinaryExpression(Syntax.BinaryExpressionSyntax syntax)
    {
        var boundLeft = BindExpression(syntax.Left);
        var boundRight = BindExpression(syntax.Right);
        var boundOperator = BinaryOperator.Bind(syntax.Token.Kind, boundLeft.Type, boundRight.Type);

        if (boundOperator == null)
        {

            _diagnostics.ReportUndefinedUnaryOperator(syntax.Token.TextSpan, syntax.Token.Text, boundLeft.Type, boundRight.Type);
            return boundLeft;
        }

        return new BinaryExpression(boundLeft, boundOperator, boundRight);
    }
}