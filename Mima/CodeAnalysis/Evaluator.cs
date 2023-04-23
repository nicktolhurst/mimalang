namespace Mima.CodeAnalysis;

using Binding;

internal class Evaluator
{
    private readonly BoundExpression _root;
    private readonly Dictionary<VariableSymbol, object> _variables;

    public Evaluator(BoundExpression root, Dictionary<VariableSymbol, object> variables)
    {
        _root = root;
        _variables = variables;
    }

    public object Evaluate()
    {
        return EvaluateExpression(_root);
    }

    private object EvaluateExpression(BoundExpression node)
    {
        if (node is BoundLiteralExpression n)
            return n.Value;

        if (node is BoundVariableExpression v)
            return _variables[v.Variable];

        if (node is BoundAssignmentExpression a)
        {
            var value = EvaluateExpression(a.Expression);
            _variables[a.Variable] = value;
            return value;
        }

        if (node is BoundUnaryExpression unaryExpression)
        {
            var operand = EvaluateExpression(unaryExpression.Operand);

            return unaryExpression.Operator.Kind switch
            {
                UnaryOperatorKind.Identity => (int) operand,
                UnaryOperatorKind.Negation => -(int) operand,
                UnaryOperatorKind.LogicalNegation => !(bool)operand,
                _ => throw new Exception($"Unexpected unary operator '{unaryExpression.Operator.Kind}'")
            };
        }

        if (node is BoundBinaryExpression binaryExpression)
        {
            var left = EvaluateExpression(binaryExpression.Left);
            var right = EvaluateExpression(binaryExpression.Right);

            return binaryExpression.Operator.Kind switch
            {
                BinaryOperatorKind.Addition => (int) left + (int) right,
                BinaryOperatorKind.Subtraction => (int) left - (int) right,
                BinaryOperatorKind.Division => (int) left / (int) right,
                BinaryOperatorKind.Multiplication => (int)left * (int)right,
                BinaryOperatorKind.LogicalAND => (bool) left && (bool) right,
                BinaryOperatorKind.LogicalOR => (bool)left || (bool)right,
                BinaryOperatorKind.Equals => Equals(left, right),
                BinaryOperatorKind.NotEquals => !Equals(left, right),
                _ => throw new Exception($"Unexpected binary operator '{binaryExpression.Operator.Kind}'")
            };
        }

        throw new Exception($"Unexpected node '{node.Kind}'");
    }
} 