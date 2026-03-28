using System;

namespace EssaLab.StronglyTypedIds.Convertors.Json.Common.Models;

internal record struct IdJsonData(string Name, string? Namespace, string BackingType) : IEquatable<IdJsonData>;