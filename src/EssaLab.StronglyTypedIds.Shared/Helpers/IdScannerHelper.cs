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

public static class IdScannerHelper
{
    public static IEnumerable<IdData> Scan(Compilation compilation)
    {
        var ids = new List<IdData>();

        // 1. Check referenced assemblies
        foreach (var reference in compilation.References)
        {
            if (compilation.GetAssemblyOrModuleSymbol(reference) is IAssemblySymbol asmSymbol)
            {
                var registryInAsm = asmSymbol.GetTypeByMetadataName(LibConstants.RegistryFullName);
                if (registryInAsm != null)
                {
                    AddIdsFromRegistry(registryInAsm, ids);
                }
            }
        }
        return ids;
    }

    private static void AddIdsFromRegistry(INamedTypeSymbol registryInAsm, List<IdData> ids)
    {
        var attributes = registryInAsm.GetAttributes()
            .Where(a => a.AttributeClass?.Name == LibConstants.RegistryAttributeName);
        
        foreach (var attribute in attributes)
        {
            if (attribute.ConstructorArguments.Length == 3)
            {
                ids.Add(new IdData(
                        attribute.ConstructorArguments[0].Value?.ToString() ?? string.Empty,
                        attribute.ConstructorArguments[1].Value?.ToString() ?? string.Empty,
                        attribute.ConstructorArguments[2].Value?.ToString() ?? "GUID"));   
            }
        }
    }
}