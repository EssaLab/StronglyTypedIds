using System;

namespace EssaLab.StronglyTypedIds.Core.Common.Models;

/// <summary>
/// Represents the metadata of a record decorated with [StronglyTypedId] attribute.
/// </summary>
/// <param name="Name">The name of the record.</param>
/// <param name="Namespace">The namespace of the record.</param>
/// <param name="BackingType">The backing type of the ID (e.g., Guid, int, long).</param>
/// <param name="HasIssue">Whether there's a diagnostic issue (like missing partial).</param>
/// <param name="Location">The location of the issue for reporting.</param>
public record struct IdRecordData(
    string Name,
    string? Namespace,
    string BackingType,
    bool HasIssue = false,
    Microsoft.CodeAnalysis.Location? Location = null) : IEquatable<IdRecordData>;
