using System;

namespace EssaLab.StronglyTypedIds.Core.Common.Models;
public record struct IdGenerationData(
    string Name,
    string FullName,
    string? Namespace,
    string BackingType,
    bool IsValueType) : IEquatable<IdGenerationData>;