using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit.TagHelpers;

[JsonSerializable(typeof(IDictionary<string, string>))]
[JsonSourceGenerationOptions(WriteIndented = false, GenerationMode = JsonSourceGenerationMode.Serialization)]
internal partial class HtmxHeaderJsonSerializerContext : JsonSerializerContext;
