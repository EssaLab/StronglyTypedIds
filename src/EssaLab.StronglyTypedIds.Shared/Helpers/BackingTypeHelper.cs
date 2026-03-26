using Microsoft.CodeAnalysis;

namespace EssaLab.StronglyTypedIds.Shared.Helpers;

internal static class BackingTypeHelper
{
    internal static string GetBackingType(this AttributeData attrData)
    {
        if (attrData.AttributeClass is { IsGenericType: true, TypeArguments.Length: > 0 })
        {
            var typeSymbol = attrData.AttributeClass.TypeArguments[0];
            return typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat).ToUpper();
        }
        return "GUID"; // Default
    }
}