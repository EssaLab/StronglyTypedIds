namespace EssaLab.StronglyTypedIds.Shared.Templates;

public interface ITypeTemplate
{
    string BackingType { get; }
    string GenerateCoreCode(string name, string ns);
    string GenerateEfCoreCode(string name, string ns);
    string GenerateJsonCode(string name, string ns);
}
