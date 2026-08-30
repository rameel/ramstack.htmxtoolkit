using System.Text.Json;
using System.Text.Json.Serialization;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit.Serialization;

/// <summary>
/// Represents a <see cref="JsonConverter{T}" /> for arrays of <see cref="HttpVerb" /> values.
/// </summary>
internal sealed class HttpVerbArrayJsonConverter : JsonConverter<HttpVerb[]>
{
    /// <inheritdoc />
    public override HttpVerb[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, HttpVerb[] value, JsonSerializerOptions options)
    {
        // NOTE: value is never null here: null-valued properties are omitted
        // by JsonIgnoreCondition.WhenWritingNull before this converter is invoked.
        writer.WriteStartArray();

        foreach (var verb in value)
            writer.WriteStringValue(verb.GetHttpVerbValue());

        writer.WriteEndArray();
    }
}
