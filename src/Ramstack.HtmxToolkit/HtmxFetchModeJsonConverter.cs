using System.Text.Json;
using System.Text.Json.Serialization;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents a <see cref="JsonConverter{T}" /> for nullable <see cref="HtmxFetchMode" /> values.
/// </summary>
internal sealed class HtmxFetchModeJsonConverter : JsonConverter<HtmxFetchMode?>
{
    /// <inheritdoc />
    public override HtmxFetchMode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, HtmxFetchMode? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.GetValueOrDefault().GetFetchModeValue());
        }
    }
}
