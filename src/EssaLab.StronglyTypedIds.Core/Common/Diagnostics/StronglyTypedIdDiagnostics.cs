using Microsoft.CodeAnalysis;

namespace EssaLab.StronglyTypedIds.Core.Common.Diagnostics;

internal static class StronglyTypedIdDiagnostics
{
    public static readonly DiagnosticDescriptor MissingPartialKeyword = new(
        id: "STID001",
        title: "Missing partial keyword",
        messageFormat: "The type '{0}' must be declared as partial to use [StronglyTypedId]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Strongly typed ID generation requires the target type to be partial.");
}
