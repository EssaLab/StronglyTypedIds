# EssaLab.StronglyTypedIds.Core

The foundational compilation tool behind the EssaLab Strongly Typed IDs framework, focusing exclusively on domain-level identifier generation.

### Responsibilities
`EssaLab.StronglyTypedIds.Core` acts as the source generation baseline for producing performant, unencumbered Domain Identifiers. It holds the following technical objectives:

* **Emitting Structural Artifacts:** Generates the `[StronglyTypedId]` marker attribute explicitly within the scope of the consuming assembly. This avoids distributing an external runtime DLL dependency just for markers.
* **Assembly Metadata Discovery (AMD):** Automatically flags the compiling assembly with `EssaLab.StronglyTypedIds.Fingerprint._StronglyTypedIdsBaseGenerated`. This metadata fingerprint plays a crucial role for advanced tooling; it permits satellite converters (such as `EssaLab.StronglyTypedIds.Convertors.EFCore`) to locate Domain Assemblies in micro-seconds, guaranteeing O(1) discovery performance and safely bypassing recursive AST node traversal operations entirely.
* **Primitive and Complex Operators:** Emits concrete `record struct` identifiers backed optionally by `Guid`, `int`, `long`, `string`, or `decimal`. Resolves `IComparable`, `IEquatable`, and mathematical equality implementations cleanly into source.
* **Universal Parameter Binding Engine:** Generates multiple universal `TryParse` definitions across compatibility boundaries. When installed on contemporary frameworks (`.NET 7+`), it synthesizes standard `IParsable<T>` representations out of the box. This provides pristine routing bindings allowing parameters to automatically map from URLs directly into Strongly Typed IDs across Minimal API and ASP.NET Core MVC controllers with zero configuration.

### Best Practices
Install this minimal dependency within your foundational mapping layers, such as `YourApplication.Domain`. Types defined internally will gracefully scale out through auxiliary compilation references down the pipeline.
