namespace Mima.CodeAnalysis;
using Mima.CodeAnalysis.Text;
public enum DiagnosticType
{
    LexicalAnalysisError,
    ParsingError,
    UnexpectedOperator,
    UnexpectedToken,
    TypeError,
    BadCharacter
}

public sealed class Diagnostic
{
    public Diagnostic(TextSpan span, string message, DiagnosticType diagnosticType)
    {
        Span = span;
        Message = message;
        DiagnosticType = diagnosticType;
    }

    public TextSpan Span { get; }
    public string Message { get; }
    public DiagnosticType DiagnosticType { get; }

    public override string ToString() => Message;
}
