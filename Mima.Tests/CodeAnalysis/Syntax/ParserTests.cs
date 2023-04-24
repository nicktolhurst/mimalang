namespace Mima.CodeAnalysis.Syntax;

public class ParserTests
{
    [Theory]
    [MemberData(nameof(GetBinaryOperatorPairsData))]
    public void Parser_BinaryExpression_HonorsPrecedences(Kind op1, Kind op2)
    {
        var op1Precedence = Facts.GetBinaryOperatorPrecedence(op1);
        var op2Precedence = Facts.GetBinaryOperatorPrecedence(op2);
        var op1Text = Facts.GetText(op1) ?? string.Empty;
        var op2Text = Facts.GetText(op2) ?? string.Empty;
        var text = $"a {op1Text} b {op2Text} c";
        var expression = SyntaxTree.Parse(text).Root;

        if (op1Precedence >= op2Precedence)
        {
            //     op2
            //    /   \
            //   op1   c
            //  /   \
            // a     b

            using var e = new AssertingEnumerator(expression);

            e.AssertNode(Kind.BinaryExpression);
            e.AssertNode(Kind.BinaryExpression);
            e.AssertNode(Kind.NameExpression);
            e.AssertToken(Kind.Identifier, "a");
            e.AssertToken(op1, op1Text);
            e.AssertNode(Kind.NameExpression);
            e.AssertToken(Kind.Identifier, "b");
            e.AssertToken(op2, op2Text);
            e.AssertNode(Kind.NameExpression);
            e.AssertToken(Kind.Identifier, "c");

        }
        else
        {
            //   op1
            //  /   \
            // a    op2
            //     /   \
            //    b     c

            using var e = new AssertingEnumerator(expression);

            e.AssertNode(Kind.BinaryExpression);
            e.AssertNode(Kind.NameExpression);
            e.AssertToken(Kind.Identifier, "a");
            e.AssertToken(op1, op1Text);
            e.AssertNode(Kind.BinaryExpression);
            e.AssertNode(Kind.NameExpression);
            e.AssertToken(Kind.Identifier, "b");
            e.AssertToken(op2, op2Text);
            e.AssertNode(Kind.NameExpression);
            e.AssertToken(Kind.Identifier, "c");
        }
    }

    [Theory]
    [MemberData(nameof(GetUnaryOperatorPairsData))]
    public void Parser_UnaryExpression_HonorsPrecedences(Kind unaryKind, Kind binaryKind)
    {
        var unaryPrecedence = Facts.GetUnaryOperatorPrecedence(unaryKind);
        var binaryPrecedence = Facts.GetBinaryOperatorPrecedence(binaryKind);
        var unaryText = Facts.GetText(unaryKind) ?? string.Empty;
        var binaryText = Facts.GetText(binaryKind) ?? string.Empty;
        var text = $"{unaryText} a {binaryText} b";
        var expression = SyntaxTree.Parse(text).Root;

        if (unaryPrecedence >= binaryPrecedence)
        {
            //   binary
            //   /    \
            // unary   b
            //   |
            //   a

            using var e = new AssertingEnumerator(expression);

            e.AssertNode(Kind.BinaryExpression);
            e.AssertNode(Kind.UnaryExpression);
            e.AssertToken(unaryKind, unaryText);
            e.AssertNode(Kind.NameExpression);
            e.AssertToken(Kind.Identifier, "a");
            e.AssertToken(binaryKind, binaryText);
            e.AssertNode(Kind.NameExpression);
            e.AssertToken(Kind.Identifier, "b");
        }
        else
        {
            //  unary
            //    |
            //  binary
            //  /   \
            // a     b

            using var e = new AssertingEnumerator(expression);
            
            e.AssertNode(Kind.UnaryExpression);
            e.AssertToken(unaryKind, unaryText);
            e.AssertNode(Kind.BinaryExpression);
            e.AssertNode(Kind.NameExpression);
            e.AssertToken(Kind.Identifier, "a");
            e.AssertToken(binaryKind, binaryText);
            e.AssertNode(Kind.NameExpression);
            e.AssertToken(Kind.Identifier, "b");
        }
    }

    public static IEnumerable<object[]> GetBinaryOperatorPairsData()
    {
        foreach (var op1 in Facts.GetBinaryOperatorsKinds())
        {
            foreach (var op2 in Facts.GetBinaryOperatorsKinds())
            {
                yield return new object[] { op1, op2 };
            }
        }
    }

    public static IEnumerable<object[]> GetUnaryOperatorPairsData()
    {
        foreach (var unary in Facts.GetUnaryOperatorsKinds())
        {
            foreach (var binary in Facts.GetBinaryOperatorsKinds())
            {
                yield return new object[] { unary, binary };
            }
        }
    }
}
