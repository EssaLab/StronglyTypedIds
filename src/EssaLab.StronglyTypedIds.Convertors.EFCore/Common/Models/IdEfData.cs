using System;

namespace EssaLab.StronglyTypedIds.Convertors.EFCore.Common.Models;

internal record struct EntityReference(string Name, string? Namespace) : IEquatable<EntityReference>;

internal record struct IdKey(string Name, string? Namespace) : IEquatable<IdKey>;

internal record struct IdEfData(
    IdKey Key,
    string BackingType
) : IEquatable<IdEfData>;
