using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents a JSON converter for <see cref="HtmxValues"/>.
/// </summary>
internal sealed class HtmxValuesJsonConverter : JsonConverter<HtmxValues>
{
    /// <inheritdoc />
    public override HtmxValues Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, HtmxValues value, JsonSerializerOptions options)
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
