using Microsoft.CodeAnalysis;

namespace EssaLab.StronglyTypedIds.Convertors.Json.Common.Diagnostics;

internal static class JsonConverterDiagnostics
{
    public static readonly DiagnosticDescriptor JsonLibraryMissing = new(
        id: "STID201",
        title: "System.Text.Json missing",
        messageFormat: "Add a reference to System.Text.Json to use JSON converters",
        category: "Setup",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The JSON converter generator requires the System.Text.Json package to be referenced.");
}
