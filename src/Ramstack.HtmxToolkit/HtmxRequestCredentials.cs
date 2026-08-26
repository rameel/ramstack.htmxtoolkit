namespace Ramstack.HtmxToolkit;

/// <summary>
/// Specifies the credentials mode for an HTMX request.
/// </summary>
public enum HtmxRequestCredentials
{
    /// <summary>
    /// Sends credentials only when the request targets the current origin.
    /// </summary>
    SameOrigin,

    /// <summary>
    /// Always sends credentials with the request.
    /// </summary>
    Include,

    /// <summary>
    /// Never sends credentials with the request.
    /// </summary>
    /// <remarks>Supported only in HTMX 4.x.</remarks>
    Omit
}
