# EssaLab.StronglyTypedIds.Convertors.EFCore

The Entity Framework Core specific source generator responsible for translating mapped Domain Identifiers into relational database primitives seamlessly.

### Responsibilities
When integrated into your **Infrastructure** or **Persistence** tier, this satellite generator synchronizes with the foundational Core generator to eliminate boilerplate Entity Framework configurations:

* **Global Compilation Discovery:** Relies on the standard `IdExtractor` from the Centralized Template Registry. Rather than analyzing `DbContext` classes or parsing individual `DbSet<T>` properties (which incurs massive performance degradation on large schemas), this generator inspects the linked Domain assembly directly for the metadata fingerprint. It efficiently identifies every `[StronglyTypedId]` declaration natively in milliseconds.
* **Deterministic Native Value Conversions:** Generates dedicated `ValueConverter<TId, TBacking>` implementations natively mapped to the underlying data store for each corresponding ID type (e.g. `Guid`, `int`, `long`, `string`, `decimal`).
* **Centralized Configuration Interface:** Emits the globally accessible extension method `AddStronglyTypedIdConventions(this ModelConfigurationBuilder)`. This method establishes a global type translation convention across the complete DbContext boundary safely.

### Best Practices
Install this NuGet package inside the project library maintaining your Entity Framework `DbContext` layer (e.g. `YourApplication.Infrastructure`). Include standard model configuration conventions as follows:

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    // Automatically aligns all Strongly Typed IDs with their respective DB primitives.
    configurationBuilder.AddStronglyTypedIdConventions();
}
```