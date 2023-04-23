namespace Mima.Tests.CodeAnalysis.Syntax;

using Mima.CodeAnalysis.Syntax;

public class FactsTests
{
    [Theory]
    [MemberData(nameof(GetKindData))]
    public void Facts_GetText_RoundTrips(Kind kind)
    {
        var text = Facts.GetText(kind);
        if(text == null)
            return;
        
        var tokens = SyntaxTree.ParseTokens(text);
        var token = Assert.Single(tokens);

        Assert.Equal(kind, token.Kind);
        Assert.Equal(text, token.Text);
    }

    public static IEnumerable<object[]> GetKindData()
    {
        var kinds = (Kind[])Enum.GetValues(typeof(Kind));
        foreach (var kind in kinds)
            yield return new object[] { kind };
    }
}


