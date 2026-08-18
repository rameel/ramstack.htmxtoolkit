using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit.TagHelpers;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(HtmxRequestTagHelper.HtmxRequestData))]
internal partial class HtmxRequestJsonSerializerContext : JsonSerializerContext;
