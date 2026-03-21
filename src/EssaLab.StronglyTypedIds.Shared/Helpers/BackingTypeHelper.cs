using Microsoft.CodeAnalysis;

namespace EssaLab.StronglyTypedIds.Shared.Helpers;

internal static class BackingTypeHelper
{
    internal static string GetBackingType(this AttributeData attrData)
    {
        if (attrData.AttributeClass is { IsGenericType: true, TypeArguments.Length: > 0 })
        {
            var typeSymbol = attrData.AttributeClass.TypeArguments[0];
        
            // دي أهم حتة: بتجبره يرجع الكلمات المحجوزة (int, string, Guid) 
            // بدلاً من الأسماء الطويلة (System.Int32)
            return typeSymbol.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
        }
    
        return "Guid"; // Default
    }
}