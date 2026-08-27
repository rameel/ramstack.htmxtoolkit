using System.Text.Json;
using System.Text.Json.Serialization;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents a <see cref="JsonConverter{T}"/> for nullable <see cref="HtmxSwap"/> values.
/// </summary>
internal sealed class HtmxSwapJsonConverter : JsonConverter<HtmxSwap?>
{
    /// <inheritdoc />
    public override HtmxSwap? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, HtmxSwap? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.GetValueOrDefault().GetSwapValue());
        }
    }
}
