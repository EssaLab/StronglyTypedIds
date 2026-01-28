# EssaLab.StronglyTypedIds.Convertors.EFCore

**EF Core Source Generator** for `EssaLab.StronglyTypedIds`. This library automates the generation of Entity Framework Core `ValueConverter`s, allowing you to use strongly-typed IDs in your entities while storing them as primitives (Guid/int/long) in the database.

## Features

- 🔄 **Auto-Generation**: Creates `ValueConverter` classes for all strongly-typed IDs found in referenced assemblies.
- 🛠️ **Bulk Configuration**: Provides a single extension method to register all converters at once.
- 🧼 **Clean Architecture**: Designed to be installed in your Infrastructure/Persistence layer, keeping your Domain layer free of EF Core dependencies.

## Installation

Install the package via NuGet in your **Infrastructure/Data Access** project:

```bash
dotnet add package EssaLab.StronglyTypedIds.Convertors.EFCore
```

> **Note**: This project requires `Microsoft.EntityFrameworkCore` to be referenced.

## Usage

### 1. Structure
Assumption: You have a **Domain** project (with IDs) and an **Infrastructure** project (with EF Core).

- **Domain Project**: Installs `EssaLab.StronglyTypedIds.Gens.Base`. Defines `UserId`.
- **Infrastructure Project**: References **Domain**. Installs `EssaLab.StronglyTypedIds.Convertors.EFCore`.

### 2. Configure `DbContext`

In your `DbContext`, override `ConfigureConventions` and call the generated extension method:

```csharp
using Microsoft.EntityFrameworkCore;
using StronglyTypedIds.EntityFrameworkCore; // Generated namespace

public class AppDbContext : DbContext
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // Automatically configures all strongly-typed IDs to use their generated ValueConverters
        configurationBuilder.AddStronglyTypedIdConventions();
        
        base.ConfigureConventions(configurationBuilder);
    }
}
```

That's it! Now EF Core will automatically handle the conversion between `UserId` <-> `Guid` (or `int`/`long`) when reading/writing to the database.

## How It Works

1. The generator scans all **referenced assemblies** for types marked with `[StronglyTypedId]`.
2. For each ID found, it generates a `ValueConverter<TId, TPrimitive>` (e.g., `UserIdEfConverter`).
3. It generates an extension method `AddStronglyTypedIdConventions` that registers all these converters using `HaveConversion`.

## Generated Code Example

For a `UserId` (Guid), it generates:

```csharp
internal sealed class UserIdEfConverter : ValueConverter<UserId, Guid>
{
    public UserIdEfConverter() : base(
        id => id.Value,          // To Provider (Write)
        value => new UserId(value)) // From Provider (Read)
    { }
}
```