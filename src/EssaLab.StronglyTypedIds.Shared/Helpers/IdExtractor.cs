using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace EssaLab.StronglyTypedIds.Shared.Helpers;

public readonly struct IdData
{
    public string Name { get; }
    public string? Namespace { get; }
    public string BackingType { get; }

    public IdData(string name, string? @namespace, string backingType)
    {
        Name = name;
        Namespace = @namespace;
        BackingType = backingType;
    }
}

public static class IdExtractor
{
    private const string FingerprintName = "_StronglyTypedIdsBaseGenerated";
    private const string AttributeName = "StronglyTypedIdAttribute";

    public static IEnumerable<IdData> ExtractIdsFromCompilation(Compilation compilation)
    {
        var ids = new List<IdData>();

        // 1. Check referenced assemblies
        foreach (var reference in compilation.References)
        {
            if (compilation.GetAssemblyOrModuleSymbol(reference) is IAssemblySymbol asmSymbol)
            {
                if (IsAssemblyMarked(asmSymbol))
                {
                    ExtractIdsFromNamespace(asmSymbol.GlobalNamespace, ids);
                }
            }
        }

        // 2. Check the current compilation's assembly
        if (IsAssemblyMarked(compilation.Assembly))
        {
            ExtractIdsFromNamespace(compilation.Assembly.GlobalNamespace, ids);
        }

        return ids;
    }

    private static bool IsAssemblyMarked(IAssemblySymbol assembly)
    {
        return assembly.GetAttributes().Any(a => 
            a.AttributeClass?.Name == FingerprintName);
    }

    private static void ExtractIdsFromNamespace(INamespaceSymbol namespaceSymbol, List<IdData> ids)
    {
        foreach (var member in namespaceSymbol.GetMembers())
        {
            if (member is INamespaceSymbol ns)
            {
                ExtractIdsFromNamespace(ns, ids);
            }
            else if (member is INamedTypeSymbol type)
            {
                ExtractIdsFromType(type, ids);
            }
        }
    }
    
    private static void ExtractIdsFromType(INamedTypeSymbol type, List<IdData> ids)
    {
        var attrData = type.GetAttributes()
            .FirstOrDefault(a => a.AttributeClass?.Name == AttributeName);
            
        if (attrData is not null)
        {
            var backing = attrData.GetBackingType();
            ids.Add(new IdData(
                type.Name, 
                type.ContainingNamespace.IsGlobalNamespace ? null : type.ContainingNamespace.ToDisplayString(), 
                backing));
        }

        foreach (var nestedType in type.GetTypeMembers())
        {
            ExtractIdsFromType(nestedType, ids);
        }
    }
}
