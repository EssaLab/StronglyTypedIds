using System.Collections.Generic;
using System.Linq;
using System.Text;
using EssaLab.StronglyTypedIds.Convertors.EFCore.Common.Diagnostics;
using EssaLab.StronglyTypedIds.Convertors.EFCore.Common.Models;
using EssaLab.StronglyTypedIds.Shared;
using EssaLab.StronglyTypedIds.Shared.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EssaLab.StronglyTypedIds.Convertors.EFCore;

/// <summary>
/// Incremental source generator for EF Core Value Converters for strongly-typed IDs.
/// </summary>
[Generator]
public sealed class EfConverterGenerator : IIncrementalGenerator
{
    private const string AttributeFullName = LibConstants.AttributeName;
    private const string FingerprintFullName = LibConstants.FingerprintName;
    private const string EfCoreNameSpace = "Microsoft.EntityFrameworkCore";
    private const string TargetPropertyIdentifierName = "DbSet";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var entityReferences = context.SyntaxProvider.CreateSyntaxProvider(
            predicate: static (node, _) => node is PropertyDeclarationSyntax { Type: GenericNameSyntax { Identifier.Text: TargetPropertyIdentifierName } },
            transform: static (ctx, _) =>
            {
                var prop = (IPropertySymbol?)ctx.SemanticModel.GetDeclaredSymbol(ctx.Node);
                var entityType = (prop?.Type as INamedTypeSymbol)?.TypeArguments.FirstOrDefault();
                if (entityType is null) return default;

                return new EntityReference(
                    entityType.Name,
                    entityType.ContainingNamespace.IsGlobalNamespace ? null : entityType.ContainingNamespace.ToDisplayString());
            })
            .Where(static r => r.Name is not null);

        var idsFromEntities = entityReferences.Combine(context.CompilationProvider)
            .SelectMany(static (data, _) =>
            {
                var (reference, compilation) = data;
                var fullName = reference.Namespace is null ? reference.Name : $"{reference.Namespace}.{reference.Name}";
                var entityType = compilation.GetTypeByMetadataName(fullName);
                if (entityType is null) return Enumerable.Empty<IdEfData>();

                return ExtractIds(entityType, compilation);
            });

        var uniqueIds = idsFromEntities.Collect()
            .Select(static (all, _) =>
            {
                var unique = all.GroupBy(x => x.Key).Select(g => g.First()).ToArray();
                return new EquatableArray<IdEfData>(unique);
            });

        var hasEfCore = context.CompilationProvider.Select(static (c, _) =>
            c.ReferencedAssemblyNames.Any(a => a.Name == EfCoreNameSpace));

        context.RegisterSourceOutput(uniqueIds.Combine(hasEfCore), static (spc, data) =>
        {
            var (ids, hasEf) = data;

            if (ids.Count == 0)
                return;

            if (!hasEf)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    EfConverterDiagnostics.EfCoreMissing,
                    Location.None));
                return;
            }

            GenerateExtensionClass(spc, ids);
            foreach (var id in ids)
            {
                GenerateStandaloneConverter(spc, id);
            }
        });
    }

    private static IEnumerable<IdEfData> ExtractIds(ITypeSymbol entity, Compilation compilation)
    {
        var attr = compilation.GetTypeByMetadataName(AttributeFullName);
        var fingerprint = compilation.GetTypeByMetadataName(FingerprintFullName);

        if (attr is null || fingerprint is null)
            yield break;

        foreach (var prop in entity.GetMembers().OfType<IPropertySymbol>())
        {
            if (prop.Type is not INamedTypeSymbol propType)
                continue;

            // Verify if the assembly of the type has the fingerprint
            if (propType.ContainingAssembly is not IAssemblySymbol asm || 
                !asm.GetAttributes().Any(a => SymbolEqualityComparer.Default.Equals(a.AttributeClass, fingerprint)))
            {
                continue;
            }
            
            //compare using  OriginalDefinition
            var attrData = propType.GetAttributes()
                .FirstOrDefault(a => a.AttributeClass is not null && 
                                     SymbolEqualityComparer.Default.Equals(a.AttributeClass.OriginalDefinition, attr));
            
            if (attrData is null)
                continue;
            
            var backing = attrData.GetBackingType();

            yield return new IdEfData(
                new IdKey(
                    propType.Name,
                    propType.ContainingNamespace.IsGlobalNamespace ? null : propType.ContainingNamespace.ToDisplayString()),
                backing);
        }
    }

    private static void GenerateExtensionClass(SourceProductionContext spc, EquatableArray<IdEfData> ids)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using Microsoft.EntityFrameworkCore;");
        sb.AppendLine();
        sb.AppendLine("namespace Microsoft.EntityFrameworkCore;");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine("/// Extension methods for registering strongly-typed ID converters in EF Core.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine("public static class StronglyTypedIdEfExtensions");
        sb.AppendLine("{");
        sb.AppendLine("    /// <summary>");
        sb.AppendLine("    /// Configures all identified strongly-typed IDs to use their corresponding value converters.");
        sb.AppendLine("    /// </summary>");
        sb.AppendLine("    public static void AddStronglyTypedIdConventions(this ModelConfigurationBuilder configurationBuilder)");
        sb.AppendLine("    {");
        foreach (var id in ids)
        {
            sb.AppendLine($"        configurationBuilder.Properties<{(id.Key.Namespace is null ? "" : id.Key.Namespace + ".")}{id.Key.Name}>().HaveConversion<{(id.Key.Namespace is null ? "" : id.Key.Namespace + ".")}{id.Key.Name}EfConverter>();");
        }
        sb.AppendLine("    }");
        sb.AppendLine("}");
        spc.AddSource("StronglyTypedIdEfExtensions.g.cs", sb.ToString());
    }

    private static void GenerateStandaloneConverter(SourceProductionContext spc, IdEfData data)
    {
        var sb = new StringBuilder();
        var converterName = $"{data.Key.Name}EfConverter";
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("#nullable enable");
        sb.AppendLine("using Microsoft.EntityFrameworkCore.Storage.ValueConversion;");
        sb.AppendLine();
        sb.AppendLine($"namespace {data.Key.Namespace ?? "EssaLab.StronglyTypedIds.Convertors.EntityFrameworkCore"};");
        sb.AppendLine();
        sb.AppendLine("/// <summary>");
        sb.AppendLine($"/// EF Core ValueConverter for <see cref=\"{(data.Key.Namespace is null ? "" : data.Key.Namespace + ".")}{data.Key.Name}\"/>.");
        sb.AppendLine("/// </summary>");
        sb.AppendLine($"internal sealed class {converterName} : ValueConverter<{data.Key.Name}, {data.BackingType}>");
        sb.AppendLine("{");
        sb.AppendLine($"    public {converterName}() : base(");
        sb.AppendLine($"        static id => id.Value,");
        sb.AppendLine($"        static value => new {data.Key.Name}(value))");
        sb.AppendLine("    { }");
        sb.AppendLine("}");

        spc.AddSource($"{(data.Key.Namespace is null ? "" : data.Key.Namespace + ".")}{data.Key.Name}.EfConverter.g.cs", sb.ToString());
    }
}
