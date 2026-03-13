namespace EssaLab.StronglyTypedIds.Convertors.EFCore.Common.Models;

internal record struct IdKey(string Name, string? Namespace);

internal record struct IdEfData(
    IdKey Key,
    string BackingType
);
