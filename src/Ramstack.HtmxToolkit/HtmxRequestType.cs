namespace Ramstack.HtmxToolkit;

/// <summary>
/// Specifies the type of content requested by an HTMX request.
/// </summary>
/// <remarks>
/// Corresponds to the <c>HX-Request-Type</c> request header.
/// </remarks>
public enum HtmxRequestType
{
    /// <summary>
    /// The request type is not specified.
    /// </summary>
    Unspecified,

    /// <summary>
    /// A full HTML page is requested.
    /// </summary>
    Full,

    /// <summary>
    /// An HTML fragment is requested.
    /// </summary>
    Partial
}
