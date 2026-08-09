#if NET8_0_OR_GREATER
using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit.TagHelpers;

[JsonSourceGenerationOptions(
    WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    GenerationMode = JsonSourceGenerationMode.Serialization)]
[JsonSerializable(typeof(HtmxConfigTagHelper.HtmxConfiguration))]
[JsonSerializable(typeof(HtmxConfigTagHelper.AntiForgeryTokenData))]
internal partial class HtmxConfigJsonSerializerContext : JsonSerializerContext;
#endif
