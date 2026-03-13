# EssaLab.StronglyTypedIds.Convertors.Json

**Automatic System.Text.Json Converters for Strongly Typed IDs**

This package extends `EssaLab.StronglyTypedIds` to provide seamless serialization and deserialization for your API layer using `System.Text.Json`.

## ✨ Features
- 🚀 **High-Performance**: Uses `Utf8JsonReader` and `Utf8JsonWriter` directly for zero-alloc serialization.
- 🛠️ **Seamless Integration**: Works out-of-the-box with ASP.NET Core controllers.
- 🏗️ **Clean Architecture**: Designed for the **API / Web** layer.

## 🏁 Quick Start

1. Install the package:
   ```bash
   dotnet add package EssaLab.StronglyTypedIds.Convertors.Json
   ```

2. Register in `Program.cs`:
   ```csharp
   using EssaLab.StronglyTypedIds.Convertors.Json;

   builder.Services.AddControllers()
       .AddJsonOptions(options =>
       {
           // Register all converters at once
           options.JsonSerializerOptions.AddStronglyTypedIdConverters();
       });
   ```

## 📜 Full Documentation
For more details, visit the [Main Repository](https://github.com/EssaLab/StronglyTypedIds).