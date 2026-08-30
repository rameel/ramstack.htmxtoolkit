using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents a <see cref="JsonConverter{T}"/> for <see cref="HtmxHistoryMode"/> values
/// written in the format expected by the <c>history</c> configuration option:
/// <see cref="HtmxHistoryMode.Enabled"/> and <see cref="HtmxHistoryMode.Disabled"/>
/// are written as booleans, while any other value is written as its lowercase
/// string representation (e.g. "reload").
/// </summary>
/// <remarks>
/// Unlike other enum values, which are serialized as strings, this one requires a custom
/// converter: <see cref="HtmxHistoryMode.Enabled"/> and <see cref="HtmxHistoryMode.Disabled"/>
/// must be written as actual JSON booleans rather than strings, since otherwise HTMX
/// would not recognize them.
/// </remarks>
internal sealed class HtmxHistoryModeJsonConverter : JsonConverter<HtmxHistoryMode?>
{
    /// <summary>
    /// Pre-encoded "reload" text; encoding it once as a static field avoids
    /// repeated UTF-8 encoding overhead on each serialization.
    /// </summary>
    private static readonly JsonEncodedText s_reload = JsonEncodedText.Encode("reload");

    /// <inheritdoc />
    public override HtmxHistoryMode? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, HtmxHistoryMode? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            switch (value.GetValueOrDefault())
            {
                case HtmxHistoryMode.Enabled:
                    writer.WriteBooleanValue(true);
                    break;
                case HtmxHistoryMode.Disabled:
                    writer.WriteBooleanValue(false);
                    break;
                default:
                    writer.WriteStringValue(s_reload);
                    break;
            }
        }
    }
}
