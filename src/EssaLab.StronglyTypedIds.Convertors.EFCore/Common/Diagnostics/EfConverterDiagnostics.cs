using Microsoft.CodeAnalysis;

namespace EssaLab.StronglyTypedIds.Convertors.EFCore.Common.Diagnostics;

internal static class EfConverterDiagnostics
{
    public static readonly DiagnosticDescriptor EfCoreMissing = new(
        id: "STID101",
        title: "Microsoft.EntityFrameworkCore missing",
        messageFormat: "Add a reference to Microsoft.EntityFrameworkCore to use EF Core Value Converters",
        category: "Setup",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "The EF Core converter generator requires the Microsoft.EntityFrameworkCore package to be referenced.");
}
