using System.Text.Json;
using System.Text.Json.Serialization;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit.Serialization;

/// <summary>
/// Represents a <see cref="JsonConverter{T}" /> for nullable <see cref="HtmxSwap" /> values.
/// </summary>
internal sealed class HtmxSwapJsonConverter : JsonConverter<HtmxSwap?>
{
    /// <inheritdoc />
    public override HtmxSwap? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, HtmxSwap? value, JsonSerializerOptions options)
    {
        // NOTE: value is never null here: null-valued properties are omitted
        // by JsonIgnoreCondition.WhenWritingNull before this converter is invoked.
        writer.WriteStringValue(value.GetValueOrDefault().GetSwapValue());
    }
}
