namespace Mima.CodeAnalysis.Syntax;

public class ParserTests
{
    [Theory]
    [MemberData(nameof(GetBinaryOperatorPairsData))]
    public void Parser_BinaryExpression_HonorsPrecedences(Kind op1, Kind op2)
    {
        var op1Precedence = Facts.GetBinaryOperatorPrecedence(op1);
        var op2Precedence = Facts.GetBinaryOperatorPrecedence(op2);
        var op1Text = Facts.GetText(op1);
        var op2Text = Facts.GetText(op2);
        var text = $"a {op1Text} b {op2Text} c";
        var expression = SyntaxTree.Parse(text).Root;

        if (op1Precedence >= op2Precedence)
        {
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
}
