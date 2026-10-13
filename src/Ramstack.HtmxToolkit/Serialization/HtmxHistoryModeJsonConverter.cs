using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

using Ramstack.HtmxToolkit.Configuration;

namespace Ramstack.HtmxToolkit.Serialization;

/// <summary>
/// Represents a <see cref="JsonConverter{T}" /> for nullable <see cref="HtmxHistoryMode" /> values.
/// </summary>
/// <remarks>
/// Writes the <c>history</c> value expected by HTMX: <see langword="true" />,
/// <see langword="false" />, or <c>"reload"</c>.
/// </remarks>
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
        Debug.Assert(value is null
            or HtmxHistoryMode.Enabled
            or HtmxHistoryMode.Disabled
            or HtmxHistoryMode.Reload);

        // NOTE: value is never null here: null-valued properties are omitted
        // by JsonIgnoreCondition.WhenWritingNull before this converter is invoked.
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
