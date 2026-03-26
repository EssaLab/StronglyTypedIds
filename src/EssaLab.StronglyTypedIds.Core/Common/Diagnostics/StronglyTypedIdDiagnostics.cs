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
    
    public static readonly DiagnosticDescriptor UnsupportedBackingType = new(
        id: "STID002",
        title: "Unsupported backing type",
        messageFormat: "The type '{0}' is not supported as a backing type for [StronglyTypedId]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
    
    public static readonly DiagnosticDescriptor MustBeRecord = new(
        id: "STID003",
        title: "Type must be a record",
        messageFormat: "The type '{0}' must be declared as a 'record' or 'record struct' to use [StronglyTypedId]",
        category: "Usage",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
