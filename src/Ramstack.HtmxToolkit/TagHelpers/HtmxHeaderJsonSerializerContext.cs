using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit.TagHelpers;

[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSourceGenerationOptions(
    WriteIndented = false,
    GenerationMode = JsonSourceGenerationMode.Serialization)]
internal partial class HtmxHeaderJsonSerializerContext : JsonSerializerContext;
