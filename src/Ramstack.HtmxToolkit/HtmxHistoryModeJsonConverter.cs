using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Converts <see cref="HtmxHistoryMode" /> values to the JSON representation expected
/// by the <c>history</c> configuration option.
/// </summary>
internal sealed class HtmxHistoryModeJsonConverter : JsonConverter<HtmxHistoryMode?>
{
    /// <summary>
    /// The pre-encoded <c>reload</c> value used to avoid repeated UTF-8 encoding during serialization.
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
