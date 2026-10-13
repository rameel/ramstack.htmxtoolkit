using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit.Serialization;

/// <summary>
/// Represents a <see cref="JsonConverter{T}" /> for the <c>TriggerSpecsCacheEnabled</c> configuration option.
/// </summary>
/// <remarks>
/// Writes the <c>triggerSpecsCache</c> value expected by HTMX:
/// an empty object when enabled, otherwise <see langword="null" />.
/// </remarks>
internal sealed class HtmxTriggerSpecsCacheJsonConverter : JsonConverter<bool?>
{
    /// <inheritdoc />
    public override bool? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, bool? value, JsonSerializerOptions options)
    {
        if (value.GetValueOrDefault())
        {
            writer.WriteStartObject();
            writer.WriteEndObject();
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
