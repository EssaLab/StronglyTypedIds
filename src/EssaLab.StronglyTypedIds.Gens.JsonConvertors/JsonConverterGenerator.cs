using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace EssaLab.StronglyTypedIds.Gens.JsonConvertors;

[Generator]
public sealed class JsonConverterGenerator : IIncrementalGenerator
{
    private const string AttributeFullName = "StronglyTypedIds.StronglyTypedIdAttribute";
    private const string FingerprintFullName = "StronglyTypedIds._StronglyTypedIdsBaseGenerated";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // 1. مزود للكشف عن وجود System.Text.Json
        var hasJsonLibrary = context.CompilationProvider.Select(static (compilation, _) =>
            compilation.ReferencedAssemblyNames.Any(ai => ai.Name.Equals("System.Text.Json", StringComparison.OrdinalIgnoreCase)));

        // 2. المزود الرئيسي: فحص الـ Compilation بالكامل لاستخراج الـ IDs من المراجع
        var idsFromReferences = context.CompilationProvider.Select(static (compilation, cancellationToken) =>
        {
            var foundIds = new List<IdJsonData>();
            
            // البحث عن رمز الأتريبيوت والبصمة لضمان أننا نبحث عن الشيء الصحيح
            var idAttrSymbol = compilation.GetTypeByMetadataName(AttributeFullName);
            var fingerprintSymbol = compilation.GetTypeByMetadataName(FingerprintFullName);

            if (idAttrSymbol is null || fingerprintSymbol is null)
                return (HasBase: false, Ids: ImmutableArray<IdJsonData>.Empty);

            // نلف على كل الـ Assembly References (زي Domain Project)
            foreach (var reference in compilation.SourceModule.ReferencedAssemblySymbols)
            {
                // هل هذا الريفرنس يحتوي على البصمة؟ (عشان منضيعش وقت في فحص System.dll وغيرها)
                var hasFingerprint = reference.GetAttributes().Any(attr => 
                    SymbolEqualityComparer.Default.Equals(attr.AttributeClass, fingerprintSymbol));

                if (!hasFingerprint) continue;

                // لو فيه البصمة، ندخل نجيب كل الـ StronglyTypedIds اللي جواه
                GetIdsFromNamespace(reference.GlobalNamespace, idAttrSymbol, foundIds);
            }

            return (HasBase: true, Ids: foundIds.ToImmutableArray());
        });

        // 3. تجميع البيانات
        var combined = idsFromReferences.Combine(hasJsonLibrary);

        context.RegisterSourceOutput(combined, static (spc, source) =>
        {
            var ids = source.Left.Ids;
            var hasJsonLib = source.Right;
            var hasBaseInRefs = source.Left.HasBase;

            // Diagnostics
            if (!hasBaseInRefs && !ids.IsDefaultOrEmpty)
            {
                 // تحذير: وجدنا IDs بس مش لاقيين البصمة (حالة نادرة بس ممكنة)
            }

            if (!hasJsonLib && !ids.IsDefaultOrEmpty)
            {
                spc.ReportDiagnostic(Diagnostic.Create(
                    new DiagnosticDescriptor("STID002", "System.Text.Json Missing", "Add System.Text.Json reference.", "Setup", DiagnosticSeverity.Error, true),
                    Location.None));
                return;
            }

            if (ids.IsDefaultOrEmpty) return;

            // 4. التوليد (نفس منطق الكود السابق)
            GenerateExtensionClass(spc, ids);
            
            foreach (var id in ids)
            {
                GenerateStandaloneConverter(spc, id);
            }
        });
    }

    // دالة مساعدة (Recursive) للبحث داخل الـ Namespaces
    private static void GetIdsFromNamespace(INamespaceSymbol namespaceSymbol, INamedTypeSymbol idAttrSymbol, List<IdJsonData> results)
    {
        // فحص الـ Types داخل الـ Namespace الحالي
        foreach (var typeSymbol in namespaceSymbol.GetTypeMembers())
        {
            var attrData = typeSymbol.GetAttributes().FirstOrDefault(attr => 
                SymbolEqualityComparer.Default.Equals(attr.AttributeClass, idAttrSymbol));

            if (attrData is not null)
            {
                // استخراج نوع الـ ID (Guid, Int, Long) من الـ Attribute Constructor
                int typeIndex = 0;
                if (attrData.ConstructorArguments.Length > 0 && attrData.ConstructorArguments[0].Value is int val)
                {
                    typeIndex = val;
                }
                else if (attrData.ConstructorArguments.Length > 0 && attrData.ConstructorArguments[0].Value is object enumVal)
                {
                    // التعامل مع الـ Enum كـ int
                    typeIndex = (int)enumVal;
                }

                string backingType = typeIndex switch { 1 => "int", 2 => "long", _ => "Guid" };
                
                results.Add(new IdJsonData(typeSymbol.Name, typeSymbol.ContainingNamespace.ToDisplayString(), backingType));
            }
        }

        // البحث في الـ Namespaces الفرعية (Recursion)
        foreach (var childNamespace in namespaceSymbol.GetNamespaceMembers())
        {
            GetIdsFromNamespace(childNamespace, idAttrSymbol, results);
        }
    }

    // ... (نفس دوال GenerateStandaloneConverter و GenerateExtensionClass السابقة)
    
    private static void GenerateExtensionClass(SourceProductionContext spc, ImmutableArray<IdJsonData> ids)
    {
        var sb = new StringBuilder();
        sb.AppendLine("// <auto-generated/>");
        sb.AppendLine("using System.Text.Json;");
        sb.AppendLine("using System.Text.Json.Serialization;");
        sb.AppendLine();
        sb.AppendLine("namespace StronglyTypedIds.Json;");
        sb.AppendLine();
        sb.AppendLine("public static class StronglyTypedIdJsonExtensions");
        sb.AppendLine("{");
        sb.AppendLine("    public static void AddStronglyTypedIdConverters(this JsonSerializerOptions options)");
        sb.AppendLine("    {");
        foreach (var id in ids)
        {
            sb.AppendLine($"        options.Converters.Add(new {id.Namespace}.{id.Name}JsonConverter());");
        }
        sb.AppendLine("    }");
        sb.AppendLine("}");
        spc.AddSource("StronglyTypedIdJsonExtensions.g.cs", sb.ToString());
    }

    private static void GenerateStandaloneConverter(SourceProductionContext spc, IdJsonData data)
    {
         // نفس كود التوليد الذي كتبناه سابقاً تماماً
         // ...
         var sb = new StringBuilder();
         var converterName = $"{data.Name}JsonConverter";
         sb.AppendLine($"// <auto-generated/>");
         sb.AppendLine($"using System;");
         sb.AppendLine($"using System.Text.Json;");
         sb.AppendLine($"using System.Text.Json.Serialization;");
         sb.AppendLine();
         sb.AppendLine($"namespace {data.Namespace ?? "StronglyTypedIds.Json"};"); // استخدم namespace الـ ID الأصلي
         sb.AppendLine();
         sb.AppendLine($"internal sealed class {converterName} : JsonConverter<{data.Name}>");
         sb.AppendLine($"{{");
         // ... Read Method ...
         sb.AppendLine($"    public override {data.Name} Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)");
         sb.AppendLine($"    {{");
         if(data.BackingType == "Guid") sb.AppendLine($"        return new {data.Name}(reader.GetGuid());");
         else if(data.BackingType == "int") sb.AppendLine($"        return new {data.Name}(reader.GetInt32());");
         else sb.AppendLine($"        return new {data.Name}(reader.GetInt64());");
         sb.AppendLine($"    }}");
         // ... Write Method ...
         sb.AppendLine($"    public override void Write(Utf8JsonWriter writer, {data.Name} value, JsonSerializerOptions options)");
         sb.AppendLine($"    {{");
         if(data.BackingType == "Guid") sb.AppendLine($"        writer.WriteStringValue(value.Value);");
         else sb.AppendLine($"        writer.WriteNumberValue(value.Value);");
         sb.AppendLine($"    }}");
         sb.AppendLine($"}}");
         
         spc.AddSource($"{data.Name}.JsonConverter.g.cs", sb.ToString());
    }
}

internal record struct IdJsonData(string Name, string? Namespace, string BackingType);