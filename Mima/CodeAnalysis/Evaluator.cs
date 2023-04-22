namespace Mima.CodeAnalysis;

using Binding;

internal class Evaluator
{
    private readonly Expression _root;

    public Evaluator(Expression root)
    {
        _root = root;
    }

    public object Evaluate()
    {
        return EvaluateExpression(_root);
    }

    private object EvaluateExpression(Expression node)
    {

        if (node is LiteralExpression literalExpression)
        {
            return literalExpression.Value;
        }

        if (node is UnaryExpression unaryExpression)
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

        if (node is BinaryExpression binaryExpression)
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