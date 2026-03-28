namespace EssaLab.StronglyTypedIds.Core.Common.Models;

public record struct IdCombinedData(
    IdGenerationData Generation,
    IdDiagnosticData Diagnostic);