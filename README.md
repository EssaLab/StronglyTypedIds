# EssaLab StronglyTypedIds

**Zero-Overhead Strongly Typed IDs for .NET**

This solution provides a comprehensive ecosystem for implementing the **Strongly Typed ID** pattern in .NET applications using Roslyn Source Generators. It prevents "Primitive Obsession" by generating type-safe, immutable ID structs that distinctively identify your domain entities, while seamlessly handling persistence (EF Core) and serialization (JSON).

## 🚀 The Three Libraries

The solution is modular to respect Clean Architecture principles:

| Package | Purpose | Layer |
|---------|---------|-------|
| **[EssaLab.StronglyTypedIds.Core](./src/EssaLab.StronglyTypedIds.Core/README.md)** | Core generator. Defines attributes and generates the ID structs/records. | **Domain** |
| **[EssaLab.StronglyTypedIds.Convertors.EFCore](./src/EssaLab.StronglyTypedIds.Convertors.EFCore/README.md)** | Generates EF Core `ValueConverter` mappings. | **Infrastructure** |
| **[EssaLab.StronglyTypedIds.Convertors.Json](./src/EssaLab.StronglyTypedIds.Convertors.Json/README.md)** | Generates `System.Text.Json` converters for API responses. | **Web/API** |

## 💡 Why Strongly Typed IDs?

### The Problem: Primitive Obsession
Using `Guid` or `int` for everything leads to bugs that are hard to spot:

```csharp
public void UpdateOrder(Guid userId, Guid orderId)
{
    // ...
}

// ❌ COMPILS FINE, BUT WRONG! Run-time bug.
service.UpdateOrder(orderId, userId); 
```

### The Solution: Strong Typing
With this library, `UserId` and `OrderId` are different types:

```csharp
public void UpdateOrder(UserId userId, OrderId orderId)
{
    // ...
}

// 🛑 COMPILER ERROR! You cannot pass an OrderId as a UserId.
// service.UpdateOrder(orderId, userId); 
```

## 🏁 Quick Start Guide

### 1. Domain Layer
Install `EssaLab.StronglyTypedIds.Core`.

```csharp
using StronglyTypedIds;

[StronglyTypedId] // Defaults to Guid
public partial record UserId;

[StronglyTypedId(IdType.Int)]
public partial record ProductId;
```

### 2. Infrastructure Layer (EF Core)
Install `EssaLab.StronglyTypedIds.Convertors.EFCore`.

```csharp
// AppDbContext.cs
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    // Auto-register all converters
    configurationBuilder.AddStronglyTypedIdConventions();
}
```

### 3. API Layer (JSON)
Install `EssaLab.StronglyTypedIds.Convertors.Json`.

```csharp
// Program.cs
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Auto-register all JSON converters
        options.JsonSerializerOptions.AddStronglyTypedIdConverters();
    });
```

## ⚠️ Requirements

- .NET 6.0 or later (Roslyn Source Generators support).
- Your consuming projects must use the C# compiler version compatible with Source Generators.
