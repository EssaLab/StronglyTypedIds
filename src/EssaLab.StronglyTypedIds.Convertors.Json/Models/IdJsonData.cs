using System;

namespace EssaLab.StronglyTypedIds.Convertors.Json.Models;

internal record struct TypeReference(string Name, string? Namespace) : IEquatable<TypeReference>;

internal record struct IdJsonData(string Name, string? Namespace, string BackingType) : IEquatable<IdJsonData>;
