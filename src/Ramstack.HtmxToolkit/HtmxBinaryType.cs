namespace Ramstack.HtmxToolkit;

/// <summary>
/// Specifies how binary data received over a WebSocket connection is represented.
/// </summary>
/// <remarks>
/// For more information, see <see href="https://developer.mozilla.org/docs/Web/API/WebSocket/binaryType">WebSocket: binaryType property</see>.
/// </remarks>
public enum HtmxBinaryType
{
    /// <summary>
    /// Represents binary data as <see href="https://developer.mozilla.org/en-US/docs/Web/API/Blob">Blob</see> objects.
    /// This is the default.
    /// </summary>
    Blob,

    /// <summary>
    /// Represents binary data as <see href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/ArrayBuffer">ArrayBuffer</see> objects.
    /// </summary>
    ArrayBuffer
}
