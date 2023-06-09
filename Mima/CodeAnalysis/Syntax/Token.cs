namespace Mima.CodeAnalysis.Syntax;
using Mima.CodeAnalysis.Text;

public sealed class Token : Node
{
    public Token(Kind kind, int position, string text, object? value)
    {
        Kind = kind;
        Position = position;
        Text = text;
        Value = value;
    }

    public override Kind Kind { get; }
    public int Position { get; }
    public string Text { get; }
    public object? Value { get; }
    public TextSpan TextSpan => new (Position, Text?.Length ?? 0);
}