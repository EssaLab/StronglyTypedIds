namespace EssaLab.StronglyTypedIds.Core.Common.Models;

public record struct IdDiagnosticData(
    bool MissingPartial,
    bool IsUnsupportedType,
    bool NotARecord,
    string Name,
    string BackingType,
    Microsoft.CodeAnalysis.Location? Location);