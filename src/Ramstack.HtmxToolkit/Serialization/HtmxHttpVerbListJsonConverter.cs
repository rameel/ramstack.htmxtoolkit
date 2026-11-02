using System.Text.Json;
using System.Text.Json.Serialization;

using Ramstack.HtmxToolkit.Configuration;
using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit.Serialization;

/// <summary>
/// Represents a <see cref="JsonConverter{T}" /> for lists of <see cref="HtmxHttpVerb" /> values.
/// </summary>
internal sealed class HtmxHttpVerbListJsonConverter : JsonConverter<IReadOnlyList<HtmxHttpVerb>>
{
    /// <inheritdoc />
    public override IReadOnlyList<HtmxHttpVerb> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, IReadOnlyList<HtmxHttpVerb> value, JsonSerializerOptions options)
    {
        // NOTE: value is never null here: null-valued properties are omitted
        // by JsonIgnoreCondition.WhenWritingNull before this converter is invoked.
        writer.WriteStartArray();

        foreach (var verb in value)
            writer.WriteStringValue(verb.GetHttpVerbValue());

        writer.WriteEndArray();
    }
}
