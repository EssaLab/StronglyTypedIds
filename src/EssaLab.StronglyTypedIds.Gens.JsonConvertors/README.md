# EssaLab.StronglyTypedIds.Gens.JsonConvertors

**JSON Source Generator** for `EssaLab.StronglyTypedIds`. This library automates the generation of `System.Text.Json` converters, ensuring your strongly-typed IDs are serialized as their underlying primitive values (e.g., `"d2.."` or `123`) instead of complex objects.

## Features

- ⚡ **Performance**: Generates high-performance `JsonConverter<T>` classes at compile time.
- 🧹 **Clean API**: IDs look like standard `Guid`s or `int`s in your JSON API responses.
- 🔌 **Auto-Registration**: Adds an extension method to register all converters in one line.

## Installation

Install the package via NuGet in your **Web/API** project:

```bash
dotnet add package EssaLab.StronglyTypedIds.Gens.JsonConvertors
```

> **Note**: This package currently supports `System.Text.Json`.

## Usage

### 1. Register Converters

In your ASP.NET Core `Program.cs` or startup code, add the converters to the `JsonSerializerOptions`:

```csharp
using StronglyTypedIds.Json; // Generated namespace

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Automatically registers all strongly-typed ID converters
        options.JsonSerializerOptions.AddStronglyTypedIdConverters();
    });
```

### 2. JSON Output

Without converters (Default behavior):
```json
{
  "id": {
    "value": "d28888e9-2ba9-473a-a40f-e38cb54f9b35"
  },
  "username": "alan_turing"
}
```

With `JsonConvertors` (Desired behavior):
```json
{
  "id": "d28888e9-2ba9-473a-a40f-e38cb54f9b35",
  "username": "alan_turing"
}
```

## How It Works

The generator scans referenced assemblies for `[StronglyTypedId]` types and generates a custom `JsonConverter` that reads/writes the underlying primitive value (`Guid`, `int`, or `long`).