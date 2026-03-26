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
        var combinedData = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                AttributeFullName,
                predicate: static (node, _) => node is  TypeDeclarationSyntax , //RecordDeclarationSyntax , // or StructDeclarationSyntax
                transform: static (ctx, _) =>
                {
                    var symbol = (INamedTypeSymbol)ctx.TargetSymbol;
                    var typeSyntax = (TypeDeclarationSyntax)ctx.TargetNode;

                    var isRecord = typeSyntax is RecordDeclarationSyntax;
                    var fullName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat).Replace("global::", "");
                    var hasPartial = typeSyntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));
                    var backingType = ctx.Attributes[0].GetBackingType();
                    var isValueType = typeSyntax is RecordDeclarationSyntax record && 
                                      record.ClassOrStructKeyword.IsKind(SyntaxKind.StructKeyword);

                    var gen = new IdGenerationData(
                        symbol.Name,
                        fullName,
                        symbol.ContainingNamespace.IsGlobalNamespace ? null : symbol.ContainingNamespace.ToDisplayString(),
                        backingType,
                        isValueType);

                    var diag = new IdDiagnosticData(
                        MissingPartial:!hasPartial,
                        IsUnsupportedType: !TemplateProvider.HasTemplateForType(backingType),
                        NotARecord: !isRecord,
                        symbol.Name,
                        backingType,
                        typeSyntax.Identifier.GetLocation()
                    );
                    
                    return new IdCombinedData(gen, diag);
                });
        
        // diagnostic branch
        context.RegisterSourceOutput(combinedData, static (spc, source) =>
        {
            if (source.Diagnostic.MissingPartial)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    StronglyTypedIdDiagnostics.MissingPartialKeyword,
                    source.Diagnostic.Location,
                    source.Diagnostic.Name));
                return;
            }
            
            if (source.Diagnostic.IsUnsupportedType)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    StronglyTypedIdDiagnostics.UnsupportedBackingType,
                    source.Diagnostic.Location,
                    source.Diagnostic.BackingType));
                return;
            }

            if (source.Diagnostic.NotARecord)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    StronglyTypedIdDiagnostics.MustBeRecord,
                    source.Diagnostic.Location,
                    source.Diagnostic.Name));
                return;
            }
            
        });
        
        // generator branch
        var generationPipeline = combinedData
            .Where(static x => x.Diagnostic is { NotARecord: false, IsUnsupportedType: false })
            .Select(static (x, _) => x.Generation)
            .Collect()
            .SelectMany(static (items,_) => items.Distinct());
        
        context.RegisterSourceOutput(generationPipeline, static (spc, data) =>
        {
            var template = TemplateProvider.GetTemplate(data.BackingType);
            if (template is null) return;
            
            var source = template.GenerateCoreCode(data.Name,data.Namespace ?? "Global", data.IsValueType);
            var hintName = $"{data.FullName.Replace(".", "_")}.StronglyTypedId.g.cs"; 
            spc.AddSource(hintName, source);
        });
    }
}
