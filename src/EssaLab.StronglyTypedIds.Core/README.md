# EssaLab.StronglyTypedIds.Core

**Incremental Source Generator for Strongly Typed IDs in .NET**

This package provides the core engine for generating type-safe, immutable ID records for your domain entities.

## ✨ Features
- 🚀 **Incremental Source Generator**: Optimized for performance with zero build-time impact.
- 🏗️ **Clean Architecture Reference**: Use this in your **Domain** layer.
- 🛠️ **Rich Functionality**: Generates `IComparable<T>`, `IEquatable<T>`, `TypeConverter`, and professional `XML Documentation`.
- 🎯 **Flexible**: Support for `Guid`, `int`, and `long` backing types.

## 🏁 Quick Start

1. Install the package:
   ```bash
   dotnet add package EssaLab.StronglyTypedIds.Core
   ```

2. Define your IDs:
   ```csharp
   using EssaLab.StronglyTypedIds.Core;

   [StronglyTypedId]
   public partial record UserId;

   [StronglyTypedId(IdType.Long)]
   public partial record OrderId;
   ```

3. Enjoy type safety:
   ```csharp
   // This will cause a compiler error if you mix them up!
   void Process(UserId userId, OrderId orderId) { ... }
   ```

## 📜 Full Documentation
For more details and integration with EF Core and JSON, visit the [Main Repository](https://github.com/EssaLab/StronglyTypedIds).
