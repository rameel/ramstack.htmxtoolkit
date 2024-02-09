using System.Text.Json;
using System.Text.Json.Serialization;

namespace Ramstack.HtmxToolkit.Internal
{
    internal static class JsonOptions
    {
        public static readonly JsonSerializerOptions CamelCase = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public static readonly JsonSerializerOptions PreserveKeyCase = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false
        };
    }
}
