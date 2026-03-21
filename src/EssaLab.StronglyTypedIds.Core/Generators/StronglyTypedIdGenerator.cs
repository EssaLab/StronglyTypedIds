using System.Linq;
using System.Text;
using EssaLab.StronglyTypedIds.Core.Common.Diagnostics;
using EssaLab.StronglyTypedIds.Core.Common.Models;
using EssaLab.StronglyTypedIds.Core.Templates;
using EssaLab.StronglyTypedIds.Shared;
using EssaLab.StronglyTypedIds.Shared.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EssaLab.StronglyTypedIds.Core.Generators;

/// <summary>
/// Incremental source generator for strongly-typed IDs.
/// </summary>
[Generator]
public sealed class StronglyTypedIdGenerator : IIncrementalGenerator
{
    private const string AttributeFullName =LibConstants.AttributeName;

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var ids = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                AttributeFullName,
                predicate: static (node, _) => node is RecordDeclarationSyntax or StructDeclarationSyntax,
                transform: static (ctx, _) =>
                {
                    var symbol = (INamedTypeSymbol)ctx.TargetSymbol;
                    var typeSyntax = (TypeDeclarationSyntax)ctx.TargetNode;

                    var hasPartial = typeSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));

                    return new IdRecordData(
                        symbol.Name,
                        symbol.ContainingNamespace.IsGlobalNamespace ? null : symbol.ContainingNamespace.ToDisplayString(),
                        ctx.Attributes[0].GetBackingType(),
                        !hasPartial,
                        typeSyntax.Identifier.GetLocation()
                    );
                });
        
        context.RegisterSourceOutput(ids, static (spc, data) =>
        {
            if (data.HasIssue)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    StronglyTypedIdDiagnostics.MissingPartialKeyword,
                    data.Location,
                    data.Name));
                return;
            }

            var source = TemplatesRepository.GetCodeForGenerated(data);
            if (source is null)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    StronglyTypedIdDiagnostics.UnsupportedBackingType,
                    data.Location,
                    data.BackingType));
                return;
            }
            spc.AddSource($"{(data.Namespace is null ? "" : data.Namespace + ".")}{data.Name}.StronglyTypedId.g.cs", source!);
        });
    }

    // private static string GetBackingType(AttributeData attrData)
    // {
    //     if (attrData.ConstructorArguments.Length == 0)
    //         return "Guid";
    //
    //     var val = attrData.ConstructorArguments[0].Value;
    //     var i = val switch
    //     {
    //         int x => x,
    //         _ => (int)val!
    //     };
    //
    //     return i switch
    //     {
    //         1 => "int",
    //         2 => "long",
    //         _ => "Guid"
    //     };
    // }

    private static void AddStronglyTypedId2(StringBuilder sb, IdRecordData data)
    {
        var name = data.Name;
        var type = data.BackingType;
        bool isGuid = type == "Guid";

        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// Represents a strongly-typed ID for <see cref=\"{name}\"/>.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"[System.ComponentModel.TypeConverter(typeof(System.ComponentModel.TypeConverter))]");
        sb.AppendLine($"public partial record {name} : IComparable<{name}>, IEquatable<{name}>");
        sb.AppendLine("{");
        
        // Value property
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Gets the underlying value of the ID.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    public {type} Value {{ get; init; }}");
        sb.AppendLine();
    
        // Constructor
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Initializes a new instance of the <see cref=\"{name}\"/> record.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    public {name}({type} value) => Value = value;");
        sb.AppendLine();

        // Factory / Empty
        if (isGuid)
        {
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// Creates a new ID with a unique Guid.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine($"    public static {name} New() => new(Guid.NewGuid());");
            sb.AppendLine();
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// Represents an empty ID.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine($"    public static {name} Empty => new(Guid.Empty);");
        }
        else
        {
            sb.AppendLine("    /// <summary>");
            sb.AppendLine("    /// Represents an empty ID.");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine($"    public static {name} Empty => new(0);");
        }

        sb.AppendLine();
        sb.AppendLine("    /// <inheritdoc />");
        sb.AppendLine("    public override string ToString() => Value.ToString();");
        sb.AppendLine();

        // Comparison
        sb.AppendLine("    /// <inheritdoc />");
        sb.AppendLine($"    public int CompareTo({name}? other)");
        sb.AppendLine(isGuid 
            ? $"        => other is null ? 1 : Value.CompareTo(other.Value);" 
            : $"        => other is null ? 1 : Value.CompareTo(other.Value);");

        sb.AppendLine();

        // Explicit interface implementation if needed or just override
        sb.AppendLine("    /// <inheritdoc />");
        sb.AppendLine($"    public virtual bool Equals({name}? other) => other is not null && Value.Equals(other.Value);");
        sb.AppendLine();
        sb.AppendLine("    /// <inheritdoc />");
        sb.AppendLine("    public override int GetHashCode() => Value.GetHashCode();");
        sb.AppendLine();

        // Operators
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Implicitly converts a <see cref=\"{name}\"/> to its underlying <see cref=\"{type}\"/>.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    public static implicit operator {type}({name} id) => id?.Value ?? {(isGuid ? "Guid.Empty" : "default")};");
        
        sb.AppendLine();
        sb.AppendLine("    /// <summary>");
        sb.AppendLine($"    /// Explicitly converts a <see cref=\"{type}\"/> to a <see cref=\"{name}\"/>.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine($"    public static explicit operator {name}({type} value) => new(value);");
        sb.AppendLine("}");
    }

    // private static void AddStronglyTypedId(StringBuilder sb, IdRecordData data)
    // {
    //     var name = data.Name;
    //     var type = data.BackingType;
    //
    //     var emptyValue = type switch
    //     {
    //         "Guid"   => "Guid.Empty",
    //         "string" => "string.Empty",
    //         "int"    => "0",
    //         "long"   => "0L",
    //         _        => "default!" 
    //     };
    //     
    //     var specificMethods = type == "Guid" 
    //         ? TemplatesRepository.GuidMethods.Replace("{Name}", name) 
    //         : string.Empty;
    //     
    //     // header
    //     sb.Append(TemplatesRepository.Header.Replace("{Namespace}", data.Namespace ?? "Global"));
    //     // body
    //     var body = TemplatesRepository.RecordBody
    //         .Replace("{Name}", name)
    //         .Replace("{BackingType}", type)
    //         .Replace("{EmptyValue}", emptyValue)
    //         .Replace("{SpecificMethods}", specificMethods);
    //     sb.AppendLine(body);
    //
    // }
}
