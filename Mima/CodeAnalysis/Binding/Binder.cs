namespace Mima.CodeAnalysis.Binding;

internal sealed class Binder 
{
    private readonly DiagnosticBag _diagnostics = new();
    private readonly Dictionary<VariableSymbol, object?> _variables;

    public Binder(Dictionary<VariableSymbol, object?> variables)
    {
        _variables = variables;
    }

    public DiagnosticBag Diagnostics => _diagnostics;

    public Dictionary<VariableSymbol, object?> Variables => _variables;

    public BoundExpression BindExpression(Syntax.ExpressionSyntax syntax)
    {
        return syntax.Kind switch
        {
            Syntax.Kind.LiteralExpression => BindLiteralExpression((Syntax.LiteralExpressionSyntax)syntax),
            Syntax.Kind.UnaryExpression => BindUnaryExpression((Syntax.UnaryExpressionSyntax)syntax),
            Syntax.Kind.BinaryExpression => BindBinaryExpression((Syntax.BinaryExpressionSyntax)syntax),
            Syntax.Kind.ParenExpression => BindParenExpression((Syntax.ParenExpressionSyntax)syntax),
            Syntax.Kind.NameExpression => BindNameExpression((Syntax.NameExpressionSyntax)syntax),
            Syntax.Kind.AssignmentExpression => BindAssignmentExpression((Syntax.AssignmentExpressionSyntax)syntax),
            
            _ => throw new Exception($"Unexpected syntax {syntax.Kind}"),
        };
    }

    private BoundExpression BindParenExpression(Syntax.ParenExpressionSyntax syntax)
    {
        return BindExpression(syntax.Expression);
    }

    private BoundExpression BindAssignmentExpression(Syntax.AssignmentExpressionSyntax syntax)
    {
        var name = syntax.Token.Text;
        var boundExpression = BindExpression(syntax.Expression);

        var existingVariable = _variables.Keys.FirstOrDefault(v => v.Name == name);
        if (existingVariable != null)
            _variables.Remove(existingVariable);

        var variable = new VariableSymbol(name, boundExpression.Type);
        _variables[variable] = null;

        return new BoundAssignmentExpression(variable, boundExpression);
    }

    private BoundExpression BindNameExpression(Syntax.NameExpressionSyntax syntax)
    {
        var name = syntax.Token.Text;
        var variable = _variables.Keys.FirstOrDefault(v => v.Name == name);

        if (variable == null)
        {
            _diagnostics.ReportUndefinedName(syntax.Token.TextSpan, name);
            return new BoundLiteralExpression(0); 
        }

        return new BoundVariableExpression(variable);
    }

    private static BoundExpression BindLiteralExpression(Syntax.LiteralExpressionSyntax syntax)
    {

        var value = syntax.Value ?? 0;
        return new BoundLiteralExpression(value);
    }

    private BoundExpression BindUnaryExpression(Syntax.UnaryExpressionSyntax syntax)
    {
        var boundOperand = BindExpression(syntax.Operand);
        var boundOperator = UnaryOperator.Bind(syntax.Token.Kind, boundOperand.Type);
        
        if(boundOperator == null)
        {
            _diagnostics.ReportUndefinedUnaryOperator(syntax.Token.TextSpan, syntax.Token.Text,  boundOperand.Type);
            return boundOperand;
        }

        return new BoundUnaryExpression(boundOperator, boundOperand);
    }

    private BoundExpression BindBinaryExpression(Syntax.BinaryExpressionSyntax syntax)
    {
        var boundLeft = BindExpression(syntax.Left);
        var boundRight = BindExpression(syntax.Right);
        var boundOperator = BinaryOperator.Bind(syntax.Token.Kind, boundLeft.Type, boundRight.Type);

        if (boundOperator == null)
        {

            _diagnostics.ReportUndefinedUnaryOperator(syntax.Token.TextSpan, syntax.Token.Text, boundLeft.Type, boundRight.Type);
            return boundLeft;
        }

        return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
    }
}