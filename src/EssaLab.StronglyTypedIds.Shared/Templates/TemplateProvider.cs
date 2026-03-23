using EssaLab.StronglyTypedIds.Shared.Templates.Types;

namespace EssaLab.StronglyTypedIds.Shared.Templates;

public static class TemplateProvider
{
    private static readonly ITypeTemplate[] Templates = new ITypeTemplate[]
    {
        new GuidTemplate(),
        new IntTemplate(),
        new LongTemplate(),
        new StringTemplate(),
        new DecimalTemplate()
    };

    public static ITypeTemplate? GetTemplate(string backingType)
    {
        foreach (var t in Templates)
        {
            if (t.BackingType == backingType) return t;
        }
        return null;
    }
}
