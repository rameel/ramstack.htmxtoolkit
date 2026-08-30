using System.Text.Json;
using System.Text.Json.Serialization;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents a <see cref="JsonConverter{T}" /> for nullable <see cref="HtmxScrollBehavior" /> values.
/// </summary>
internal sealed class HtmxScrollBehaviorJsonConverter : JsonConverter<HtmxScrollBehavior?>
{
    /// <inheritdoc />
    public override HtmxScrollBehavior? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, HtmxScrollBehavior? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStringValue(value.GetValueOrDefault().GetScrollBehaviorValue());
        }
    }
}
