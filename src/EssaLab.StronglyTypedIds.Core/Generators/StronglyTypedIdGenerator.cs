using System.Linq;
using System.Text;
using EssaLab.StronglyTypedIds.Core.Common.Diagnostics;
using EssaLab.StronglyTypedIds.Core.Common.Models;
using EssaLab.StronglyTypedIds.Shared;
using EssaLab.StronglyTypedIds.Shared.Helpers;
using EssaLab.StronglyTypedIds.Shared.Templates;
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
                predicate: static (node, _) => node is RecordDeclarationSyntax , // or StructDeclarationSyntax
                transform: static (ctx, _) =>
                {
                    var symbol = (INamedTypeSymbol)ctx.TargetSymbol;
                    var typeSyntax = (TypeDeclarationSyntax)ctx.TargetNode;

                    var hasPartial = typeSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
                    var isValueType = typeSyntax is RecordDeclarationSyntax record && 
                                      record.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword);

                    return new IdRecordData(
                        symbol.Name,
                        symbol.ContainingNamespace.IsGlobalNamespace ? null : symbol.ContainingNamespace.ToDisplayString(),
                        ctx.Attributes[0].GetBackingType(),
                        !hasPartial,
                        isValueType,
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

            var template = TemplateProvider.GetTemplate(data.BackingType);
            if (template is null)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    StronglyTypedIdDiagnostics.UnsupportedBackingType,
                    data.Location,
                    data.BackingType));
                return;
            }
            var source = template.GenerateCoreCode(data.Name,data.Namespace ?? "Global", data.IsValueType);
            spc.AddSource($"{(data.Namespace is null ? "" : data.Namespace + ".")}{data.Name}.StronglyTypedId.g.cs", source);
        });
    }
}
