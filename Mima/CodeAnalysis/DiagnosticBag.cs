using System.Collections;
using System.Collections.Generic;

namespace Mima.CodeAnalysis;

internal sealed class DiagnosticBag : IEnumerable<Diagnostic>
{
    private readonly List<Diagnostic> _diagnostics = new();

    public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

    public void Report(TextSpan span, string message)
    {
        var diagnostic = new Diagnostic(span, message);

        _diagnostics.Add(diagnostic);
    }

    internal void AddRange(DiagnosticBag diagnostics)
    {
        _diagnostics.AddRange(diagnostics._diagnostics);
    }

    internal void ReportBadCharacter(int position, char current)
        => Report(new TextSpan(position, 1), $"Bad character input: '{current}'.");

    internal void ReportInvalidNumber(TextSpan span, string text, Type type) 
        => Report(span, $"The number {text} is not a valid {type}.");

    internal void ReportUndefinedName(TextSpan span, string name)
        => Report(span, $"variable '{name}' does not exist.");

    internal void ReportUndefinedUnaryOperator(TextSpan span, string text, Type type)
        => Report(span, $"Unary operator '{text}' is not defined for type '{type}'.");

    internal void ReportUndefinedUnaryOperator(TextSpan span, string text, Type leftType, Type rightType)
        => Report(span, $"BINDER: binary operator '{text}' is not defined for types '{leftType}' and '{rightType}'.");

    internal void ReportUnexpectedToken(TextSpan span, Syntax.Kind actualKind, Syntax.Kind expectedKind)
        => Report(span, $"Unexpected token <{actualKind}>, expected <{expectedKind}>.");

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
