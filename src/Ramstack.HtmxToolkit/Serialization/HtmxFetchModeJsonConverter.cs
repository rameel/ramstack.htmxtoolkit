using System.Text.Json;
using System.Text.Json.Serialization;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit.Serialization;

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
        // NOTE: value is never null here: null-valued properties are omitted
        // by JsonIgnoreCondition.WhenWritingNull before this converter is invoked.
        writer.WriteStringValue(value.GetValueOrDefault().GetFetchModeValue());
    }
}
