namespace Mima.CodeAnalysis.Syntax;

public static class Facts 
{
    public static int GetUnaryOperatorPrecedence(this Kind kind)
    {
        return kind switch
        {
            Kind.Plus or
            Kind.Minus or
            Kind.Bang => 6,

            _ => 0,
        };
    }

    public static int GetBinaryOperatorPrecedence(this Kind kind)
    {
        return kind switch
        {
            Kind.Asterisk or
            Kind.ForwardSlash => 5,

            Kind.Plus or
            Kind.Minus => 4,

            Kind.EqualsEquals or
            Kind.BangEquals => 3,

            Kind.AmpAmp => 2,
            Kind.PipePipe => 1,

            _ => 0,
        };
    }

    public static Kind GetKeywordKind(string text)
    {
        return text switch
        {
            "true" => Kind.True,
            "false" => Kind.False,
            _ => Kind.Identifier,
        };
    }

    public static string? GetText(Kind kind)
    {
        return kind switch
        {
            Kind.AmpAmp         => "&&",
            Kind.PipePipe       => "||",
            Kind.Equals         => "=",
            Kind.Bang           => "!",
            Kind.EqualsEquals   => "==",
            Kind.BangEquals     => "!=",
            // case Kind.Identifier: return "";
            // case Kind.Number: return "";
            Kind.Asterisk       => "*",
            Kind.ForwardSlash   => "/",
            Kind.Plus           => "+",
            Kind.Minus          => "-",
            Kind.CloseParen     => ")",
            Kind.OpenParen      => "(",
            Kind.False          => "false",
            Kind.True           => "true",
            _ => null,
        };
    }
}