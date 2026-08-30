using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Converts the boolean <c>triggerSpecsCache</c> abstraction to its HTMX JSON representation.
/// </summary>
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
