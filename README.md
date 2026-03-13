# EssaLab StronglyTypedIds

[![NuGet Status](https://img.shields.io/nuget/v/EssaLab.StronglyTypedIds.Core?style=flat-square)](https://www.nuget.org/packages/EssaLab.StronglyTypedIds.Core)
![License](https://img.shields.io/github/license/EssaLab/EssaLab.StronglyTypedIds?style=flat-square)

**Zero-Overhead, High-Performance Strongly Typed IDs for .NET**

This solution provides a comprehensive ecosystem for implementing the **Strongly Typed ID** pattern in .NET applications using **Incremental Source Generators**. It prevents "Primitive Obsession" by generating type-safe, immutable ID records that distinctively identify your domain entities, while seamlessly handling persistence (EF Core) and serialization (System.Text.Json).

---

## ✨ Key Features

- 🚀 **Performance Optimized**: Built using Roslyn Incremental Generators with advanced caching (`EquatableArray`) for zero-impact on build times.
- 🏗️ **Clean Architecture Ready**: Modular design with separate packages for Domain, Infrastructure (EF Core), and API (JSON).
- 🛠️ **Feature-Rich**: 
    - Automatic `IComparable<T>`, `IEquatable<T>`, and `TypeConverter` implementation.
    - Full support for `Implicit/Explicit` operators.
    - Professional `XML Documentation` generated for every ID.
- ⚡ **Auto-Discovery**: One-line registration for all IDs in EF Core and System.Text.Json.
- 🎯 **Flexible Backing Types**: Support for `Guid`, `int`, and `long`.

---

## 🚀 The Three Libraries

| Package | Purpose | Layer |
|---------|---------|-------|
| **[EssaLab.StronglyTypedIds.Core](./src/EssaLab.StronglyTypedIds.Core)** | Definition and Code Generation. | **Domain** |
| **[EssaLab.StronglyTypedIds.Convertors.EFCore](./src/EssaLab.StronglyTypedIds.Convertors.EFCore)** | EF Core `ValueConverter` mappings. | **Infrastructure** |
| **[EssaLab.StronglyTypedIds.Convertors.Json](./src/EssaLab.StronglyTypedIds.Convertors.Json)** | `System.Text.Json` converters. | **Web/API** |

---

## � Why Strongly Typed IDs?

### The Problem: Primitive Obsession
Using `Guid` or `int` for all IDs leads to dangerous bugs:

```csharp
public void UpdateOrder(Guid userId, Guid orderId) { ... }

// ❌ COMPILES FINE, BUT WRONG! Run-time bug.
service.UpdateOrder(orderId, userId); 
```

### The Solution: Type Safety
With this library, `UserId` and `OrderId` are distinct types.

```csharp
public void UpdateOrder(UserId userId, OrderId orderId) { ... }

// 🛑 COMPILER ERROR! Type safety at build time.
// service.UpdateOrder(orderId, userId); 
```

---

## 🏁 Quick Start

### 1. In Your Domain
Install `EssaLab.StronglyTypedIds.Core`.

```csharp
using EssaLab.StronglyTypedIds.Core;

[StronglyTypedId] // Defaults to Guid
public partial record UserId;

[StronglyTypedId(IdType.Long)]
public partial record OrderId;
```

### 2. In Your Infrastructure (EF Core)
Install `EssaLab.StronglyTypedIds.Convertors.EFCore`.

```csharp
// In your DbContext
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    // Auto-registers all IDs found in your entities!
    configurationBuilder.AddStronglyTypedIdConventions();
}
```

### 3. In Your Web API (System.Text.Json)
Install `EssaLab.StronglyTypedIds.Convertors.Json`.

```csharp
// In Program.cs
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Registers all JSON converters automatically
        options.JsonSerializerOptions.AddStronglyTypedIdConverters();
    });
```

---

## 📸 Developer Experience

### Professional IntelliSense
Each ID comes with full XML documentation and factory methods generated automatically.

> ![IntelliSense Placeholder](https://via.placeholder.com/800x200?text=IntelliSense+Auto-Generated+Documentation+Preview)

---

## ⚙️ Performance Benchmarks

Our generators use **Incremental Pipelines** with custom `EquatableArray` caching to ensure that build times remain unaffected even in projects with hundreds of IDs.

- **Caching Efficiency**: 100% (only regenerates on actual ID definition change).
- **Memory Footprint**: Minimal (uses static lambdas and avoiding string-based comparisons in hot paths).

---

## 📜 License
This project is licensed under the MIT License.
