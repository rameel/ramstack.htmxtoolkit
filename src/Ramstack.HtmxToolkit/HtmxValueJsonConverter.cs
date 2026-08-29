using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents a JSON converter for <see cref="HtmxValue"/>.
/// </summary>
internal sealed class HtmxValueJsonConverter : JsonConverter<HtmxValue>
{
    /// <inheritdoc />
    public override HtmxValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, HtmxValue value, JsonSerializerOptions options)
    {
        var values = value.Values;
        switch (values)
        {
            case string s:
                writer.WriteStringValue(s);
                break;

            default:
                writer.WriteStartArray();

                if (values is not null)
                    foreach (var s in Unsafe.As<string[]>(values))
                        writer.WriteStringValue(s);

                writer.WriteEndArray();
                break;
        }
    }
}
