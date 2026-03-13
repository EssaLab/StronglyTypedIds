# EssaLab.StronglyTypedIds.Convertors.EFCore

**Automatic EF Core Value Converters for Strongly Typed IDs**

This package extends `EssaLab.StronglyTypedIds` to provide seamless integration with Entity Framework Core through automatic `ValueConverter` generation.

## ✨ Features
- ⚡ **Auto-Discovery**: Automatically finds and configures converters for all your strongly typed IDs used in your entities.
- 🛠️ **Convention-Based**: One-line registration in your `DbContext`.
- 🏗️ **Clean Architecture**: Designed for the **Infrastructure** layer.

## 🏁 Quick Start

1. Install the package:
   ```bash
   dotnet add package EssaLab.StronglyTypedIds.Convertors.EFCore
   ```

2. Register in your `DbContext`:
   ```csharp
   using EssaLab.StronglyTypedIds.Convertors.EntityFrameworkCore;

   public class AppDbContext : DbContext
   {
       protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
       {
           // This one line handles everything!
           configurationBuilder.AddStronglyTypedIdConventions();
       }
   }
   ```

## 📜 Full Documentation
For more details, visit the [Main Repository](https://github.com/EssaLab/StronglyTypedIds).