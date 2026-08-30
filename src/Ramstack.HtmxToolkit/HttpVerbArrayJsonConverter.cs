using System.Text.Json;
using System.Text.Json.Serialization;

using Ramstack.HtmxToolkit.Internal;

namespace Ramstack.HtmxToolkit;

/// <summary>
/// Represents a <see cref="JsonConverter{T}" /> for arrays of <see cref="HttpVerb" /> values.
/// </summary>
internal sealed class HttpVerbArrayJsonConverter : JsonConverter<HttpVerb[]>
{
    /// <inheritdoc />
    public override HttpVerb[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        throw new NotSupportedException();

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, HttpVerb[]? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
        }
        else
        {
            writer.WriteStartArray();

            foreach (var verb in value)
                writer.WriteStringValue(verb.GetHttpVerbValue());

            writer.WriteEndArray();
        }
    }
}
