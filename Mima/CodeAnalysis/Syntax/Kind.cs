namespace Mima.CodeAnalysis.Syntax;

public enum Kind
{
    Number,
    WhiteSpace,
    Plus,
    Minus,
    OpenParen,
    CloseParen,
    BadToken,
    EOF,
    NumberExp,
    NumberExpression,
    BinaryExpression,
    OpenCurly,
    CloseCurly,
    OpenSquare,
    CloseSquare,
    ParenExpression,
    LiteralExpression,
    UnaryExpression,
    Identifier,
    True,
    False,
    Bang,
    EqualsEquals,
    PipePipe,
    AmpAmp,
    ForwardSlash,
    Asterisk,
    BangEquals
}