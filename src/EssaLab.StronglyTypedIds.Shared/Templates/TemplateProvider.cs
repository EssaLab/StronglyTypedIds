using System.Collections.Immutable;
using EssaLab.StronglyTypedIds.Shared.Templates.Types;

namespace EssaLab.StronglyTypedIds.Shared.Templates;

public static class TemplateProvider
{
    private static readonly ImmutableDictionary<string, ITypeTemplate> Templates = 
        ImmutableDictionary<string, ITypeTemplate>.Empty
            .Add("GUID", new GuidTemplate())
            .Add("INT", new IntTemplate())
            .Add("LONG", new LongTemplate())
            .Add("STRING", new StringTemplate())
            .Add("DECIMAL", new DecimalTemplate());


    public static ITypeTemplate? GetTemplate(string backingType)
    {
        return Templates.TryGetValue(backingType, out var template) ? template : null;
    }
    
    public static bool HasTemplateForType(string backingType)
    {
        return Templates.ContainsKey(backingType);
    }
}
