namespace Mima.CodeAnalysis.Text;
public sealed partial class SourceText
{
    public sealed class TextLine
    {
        public TextLine(SourceText text, int start, int length, int lengthWithLineBreak)
        {
            Text = text;
            Start = start;
            Length = length;
            LengthWithLineBreak = lengthWithLineBreak;
        }

        public SourceText Text { get; }
        public int Start { get; }
        public int Length { get; }
        public int LengthWithLineBreak { get; }
        public int End => Start + Length;
        public TextSpan Span => new(Start, Length);
        public TextSpan SpanWithLineBreak => new(Start, LengthWithLineBreak);
        
        public override string ToString() => Text.ToString(Span);
    }
}
