# EssaLab.StronglyTypedIds.Convertors.Json

The `System.Text.Json` source generator providing reflection-free JSON interoperability for EssaLab Strongly Typed IDs.

### Responsibilities
Operating gracefully within ASP.NET Core presentation architectures, this library enforces high-performance and resilient serialization pipelines:

* **Automated Type-Safe Serialization:** Replaces generic string or object mapping operations with robust, pre-compiled `JsonConverter<T>` classes tailored exactly to the target structure.
* **Zero-Reflection Overhead:** Serialization configurations are deeply integrated during Roslyn Source Generation phases. This strictly removes runtime reflection penalties conventionally associated with custom JsonConverters, maximizing Web API throughput requests.
* **Metadata Fingerprint Detection:** Bypasses naive API Controller scanning mechanisms. The generator interacts with dependent Assembly Metadata Fingerprints to effortlessly discover Domain Level IDs regardless of where or whether they appear strictly in `[ApiController]` surface DTOs.
* **Centralized Extensibility:** Produces the globally addressable extension `AddStronglyTypedIdConverters(this JsonSerializerOptions)`. This configures the entire HTTP pipeline to comprehend Domain IDs reliably.

### Best Practices
Install this package specifically in projects managing external API configuration and presentation routing (e.g., `YourApplication.WebApi` or `YourApplication.Api`). Configure your host services once to apply the mapping:

```csharp
builder.Services.ConfigureHttpJsonOptions(options =>
{
    // Injects all Strongly Typed ID serializers seamlessly
    options.SerializerOptions.AddStronglyTypedIdConverters();
});
```