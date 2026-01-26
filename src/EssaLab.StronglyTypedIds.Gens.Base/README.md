# EssaLab.StronglyTypedIds.Gens.Base

**Core Source Generator** for creating strongly-typed IDs in .NET using the "Strongly Typed ID" pattern. This library automates the creation of immutable, comparable, and type-safe ID structs/records to prevent primitive obsession (e.g., mixing up `UserId` and `OrderId`).

## Features

- 🚀 **Zero Runtime Overhead**: Uses Roslyn Source Generators to create code at design time.
- 🛡️ **Type Safety**: Impossible to accidentally swap method arguments (e.g., passing a `UserId` where an `OrderId` is expected).
- 💾 **Flexible Backing Types**: Supports `Guid` (default), `int`, and `long`.
- 🔗 **Rich Integration**: Generated IDs implement `IComparable`, `ToString`, and type conversion operators.
- 📦 **DDD Ready**: Perfect for Domain-Driven Design entities.

## Installation

Install the package via NuGet:

```bash
dotnet add package EssaLab.StronglyTypedIds.Gens.Base
```

## Usage

### 1. Basic Usage (Guid)

Simply mark a `partial record` with the `[StronglyTypedId]` attribute. The default backing type is `Guid`.

```csharp
using StronglyTypedIds;

[StronglyTypedId]
public partial record UserId;

// Usage
var id = UserId.New(); // Generates a new random Guid
```

### 2. Using Integers or Longs

You can specify the backing type in the attribute constructor:

```csharp
using StronglyTypedIds;

[StronglyTypedId(IdType.Int)]
public partial record OrderId;

[StronglyTypedId(IdType.Long)]
public partial record ProductId;

// Usage
var orderId = new OrderId(123);
var productId = new ProductId(999999999);
```

## Generated Code

The generator automatically adds the following members to your record:

- `Value` property (the underlying primitive).
- **Constructors** for creating instances.
- **Factory Methods**: `New()` for Guids, `Empty` for numeric types.
- **Conversions**: Implicit conversion to the primitive type, explicit conversion from the primitive type.
- **IComparable**: Implements `IComparable<T>` for sorting.
- **ToString()**: Overrides `ToString()` to check the inner value.

## Why Use This?

Without strongly-typed IDs:

```csharp
public void ProcessOrder(Guid userId, Guid orderId) { ... }

// ❌ Easy to mix up arguments!
ProcessOrder(orderId, userId); 
```

With strongly-typed IDs:

```csharp
public void ProcessOrder(UserId userId, OrderId orderId) { ... }

// ✅ Compiler error if you mix them up!
ProcessOrder(orderId, userId); 
```
