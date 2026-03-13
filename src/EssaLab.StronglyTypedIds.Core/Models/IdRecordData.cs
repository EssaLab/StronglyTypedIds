using System;

namespace EssaLab.StronglyTypedIds.Core.Models;

/// <summary>
/// Represents the metadata of a record decorated with [StronglyTypedId] attribute.
/// </summary>
/// <param name="Name">The name of the record.</param>
/// <param name="Namespace">The namespace of the record.</param>
/// <param name="BackingType">The underlying type (Guid, int, or long).</param>
internal record struct IdRecordData(string Name, string? Namespace, string BackingType) : IEquatable<IdRecordData>;
