namespace Ramstack.HtmxToolkit;

/// <summary>
/// Defines the <a href="https://developer.mozilla.org/docs/Web/API/WebSocket/binaryType">type of binary data</a>
/// being received over the WebSocket connection.
/// </summary>
/// <remarks>
/// https://developer.mozilla.org/docs/Web/API/WebSocket/binaryType
/// </remarks>
public enum HtmxBinaryType
{
    /// <summary>
    /// Use <a href="https://developer.mozilla.org/en-US/docs/Web/API/Blob">Blob</a> objects for binary data.
    /// This is the default value.
    /// </summary>
    Blob,

    /// <summary>
    /// Use <a href="https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/ArrayBuffer">ArrayBuffer</a>
    /// objects for binary data.
    /// </summary>
    ArrayBuffer
}
