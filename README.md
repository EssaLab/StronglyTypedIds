<div align="center">
  <img src="/docs/assets/banner.png" alt="EssaLab.StronglyTypedIds Logo" width="150"/>

  # EssaLab.StronglyTypedIds
  
  [![NuGet Version (Core)](https://img.shields.io/nuget/v/EssaLab.StronglyTypedIds.Core?style=flat-square&color=blue&label=Core)](https://www.nuget.org/packages/EssaLab.StronglyTypedIds.Core)
  [![NuGet Version (EF Core)](https://img.shields.io/nuget/v/EssaLab.StronglyTypedIds.Convertors.EFCore?style=flat-square&color=blue&label=EF%20Core)](https://www.nuget.org/packages/EssaLab.StronglyTypedIds.Convertors.EFCore)
  [![NuGet Version (JSON)](https://img.shields.io/nuget/v/EssaLab.StronglyTypedIds.Convertors.Json?style=flat-square&color=blue&label=JSON)](https://www.nuget.org/packages/EssaLab.StronglyTypedIds.Convertors.Json)
  
  [![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](https://opensource.org/licenses/MIT)
  [![.NET](https://img.shields.io/badge/.NET-6.0%20%7C%207.0%20%7C%208.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
</div>

---


A highly performant, modular source generator ecosystem for implementing Strongly Typed IDs in .NET applications.

This project uses modern Roslyn Incremental Source Generators to abstract away the boilerplate associated with Value Objects, Entity Framework Core value conversions, and System.Text.Json serialization.

## Key Architectural Advantages

**1. Unparalleled Discovery Performance**  
Traditional source generators typically rely on exhaustive Abstract Syntax Tree (AST) traversal to find where IDs are consumed (e.g., inside DTO profiles or `DbSet` configurations). This often leads to severe compilation bottlenecks in large solutions.
`EssaLab.StronglyTypedIds` resolves this by utilizing an Assembly-wide Metadata Discovery model. The core generator leaves a persistent metadata "Fingerprint" on domain assemblies. Any dependent generator (JSON, EF Core) simply scans the reference assemblies for this fingerprint and instantly enumerates the ID declarations with zero deep-tree traversing. This guarantees O(1) performance scaling regardless of how large the application layer becomes.

**2. Native ASP.NET Core Route Binding**  
All generated Strongly Typed IDs natively include robust `TryParse` method signatures and, when targeting contemporary frameworks (.NET 7+), standard `IParsable<T>` interface implementations. This completely negates the need for custom model binders or routing constraints. 
Minimal APIs and MVC Controllers identify the IDs as native uncomplex types and can perfectly bind them straight from URL parameters (e.g., `FromRoute` queries such as `/api/customers/{customerId}`).

**3. Centralized Modular Templates**  
Code generation logic is decoupled from direct string manipulations. The library operates over a Centralized Template Registry housed in the `Shared` project tier. The registry maintains a single source of truth for every backing type (`Guid`, `int`, `long`, `decimal`, `string`), distributing the required structural definitions and database mapping rules strictly to the components that need them.

## Sub-Libraries Overview

The solution is divided into autonomous but interconnected compilation tools:

- **EssaLab.StronglyTypedIds.Core:** Responsible for emitting the domain `[StronglyTypedId]` attribute, applying assembly fingerprints, and generating the baseline `record struct` components including standard equality operators.
- **EssaLab.StronglyTypedIds.Convertors.EFCore:** Automatically locates all strongly typed IDs within the domain and synthesizes Entity Framework Core `ValueConverter` mappings, providing a single `.AddStronglyTypedIdConventions()` bridge.
- **EssaLab.StronglyTypedIds.Convertors.Json:** Generates strictly-typed `JsonConverter<T>` configurations, alongside a centralized `.AddStronglyTypedIdConverters()` extension for optimal serialization.

## Getting Started

1. Decorate your records or structs in your Domain project with the base attribute:
   ```csharp
   [StronglyTypedId<Guid>]
   public partial record struct OrderId;
   ```

2. Register the Entity Framework Core configuration implicitly during `DbContext` initialization:
   ```csharp
   protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
   {
       configurationBuilder.AddStronglyTypedIdConventions();
   }
   ```

3. Register the JSON Serialization options during your API host construction:
   ```csharp
   builder.Services.ConfigureHttpJsonOptions(options =>
   {
       options.SerializerOptions.AddStronglyTypedIdConverters();
   });
   ```
