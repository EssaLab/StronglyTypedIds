using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using EssaLab.StronglyTypedIds.Core.Generators;
using Xunit;
using System.Linq;

namespace EssaLab.StronglyTypedIds.Tests;

public class GeneratorTests
{
    [Fact]
    public void Should_Handle_Duplicate_Names_In_Different_Namespaces()
    {
        // Arrange
        var source = @"
using EssaLab.StronglyTypedIds.Core;

namespace NamespaceA
{
    [StronglyTypedId]
    public partial record OrderId;
}

namespace NamespaceB
{
    [StronglyTypedId]
    public partial record OrderId;
}
";
        var compilation = CreateCompilation(source);
        var generator = new StronglyTypedIdGenerator();
        
        var driver = CSharpGeneratorDriver.Create(generator);

        // Act
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out var diagnostics);

        // Assert
        Assert.Empty(diagnostics);
        
        var generatedFiles = outputCompilation.SyntaxTrees
            .Where(x => x.FilePath.Contains("OrderId.g.cs"))
            .ToList();

        // Should generate two files with unique names (hint names)
        Assert.Equal(2, generatedFiles.Count);
        
        var contentA = generatedFiles[0].ToString();
        var contentB = generatedFiles[1].ToString();

        Assert.Contains("namespace NamespaceA", contentA);
        Assert.Contains("namespace NamespaceB", contentB);
    }

    private static Compilation CreateCompilation(string source)
    {
        return CSharpCompilation.Create("TestAssembly",
            new[] { CSharpSyntaxTree.ParseText(source) },
            new[]
            {
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(StronglyTypedIdGenerator).Assembly.Location),
                MetadataReference.CreateFromFile(System.IO.Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "netstandard.dll")),
                MetadataReference.CreateFromFile(System.IO.Path.Combine(System.Runtime.InteropServices.RuntimeEnvironment.GetRuntimeDirectory(), "System.Runtime.dll")),
            },
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
    }
}
