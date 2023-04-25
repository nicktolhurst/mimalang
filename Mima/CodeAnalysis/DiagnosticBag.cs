using System.Collections;
using Mima.CodeAnalysis.Text;

namespace Mima.CodeAnalysis;

internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
{
    private readonly List<Diagnostic> _diagnostics = new();

    public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

    public void Report(TextSpan span, string message, DiagnosticType diagnosticType)
    {
        var diagnostic = new Diagnostic(span, message, diagnosticType);

        _diagnostics.Add(diagnostic);
    }

    internal void AddRange(DiagnosticBag diagnostics)
    {
        _diagnostics.AddRange(diagnostics._diagnostics);
    }

    internal void ReportBadCharacter(int position, char current)
        => Report(new TextSpan(position, 1), $"Bad character input: '{current}'.", DiagnosticType.ParsingError);

    internal void ReportInvalidNumber(TextSpan span, string text, Type type) 
        => Report(span, $"The number {text} is not a valid {type}.", DiagnosticType.TypeError);

    internal void ReportUndefinedName(TextSpan span, string name)
        => Report(span, $"Variable '{name}' does not exist.", DiagnosticType.ParsingError);

    internal void ReportUndefinedUnaryOperator(TextSpan span, string text, Type type)
        => Report(span, $"Unary operator '{text}' is not defined for type '{type}'.", DiagnosticType.UnexpectedOperator);

    internal void ReportUndefinedUnaryOperator(TextSpan span, string text, Type leftType, Type rightType)
        => Report(span, $"Binary operator '{text}' is not defined for types '{leftType}' and '{rightType}'.", DiagnosticType.UnexpectedOperator);

    internal void ReportUnexpectedToken(TextSpan span, Syntax.Kind actualKind, Syntax.Kind expectedKind)
        => Report(span, $"Unexpected token <{actualKind}>, expected <{expectedKind}>.", DiagnosticType.LexicalAnalysisError);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
