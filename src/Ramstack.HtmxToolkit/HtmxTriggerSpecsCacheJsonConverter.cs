using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents a <see cref="JsonConverter{T}"/> that serializes the <c>triggerSpecsCache</c>
/// configuration option from its boolean form: <see langword="true" /> is written as an empty
/// JSON object (<c>{}</c>), instructing HTMX to use a never-clearing trigger specification cache,
/// while <see langword="false" /> and <see langword="null" /> are written as JSON null.
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
