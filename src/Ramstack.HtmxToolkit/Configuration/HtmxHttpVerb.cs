namespace Ramstack.HtmxToolkit.Configuration;

/// <summary>
/// Defines the HTTP methods that can be used in HTMX configuration.
/// </summary>
/// <remarks>
/// <para>
///   <c>HEAD</c> and <c>OPTIONS</c> have no <c>hx-*</c> attribute and are available only through <c>htmx.ajax</c>.
/// </para>
/// <para>
///   There are no members for <c>CONNECT</c> and <c>TRACE</c>, because browsers do not allow requests with these methods.
/// </para>
/// </remarks>
public enum HtmxHttpVerb
{
    /// <summary>
    /// The <c>GET</c> method requests a representation of the specified resource.
    /// </summary>
    Get,

    /// <summary>
    /// The <c>HEAD</c> method requests a response identical to a <c>GET</c> response,
    /// but without the response body.
    /// </summary>
    Head,

    /// <summary>
    /// The <c>POST</c> method submits an entity to the specified resource.
    /// </summary>
    Post,

    /// <summary>
    /// The <c>PUT</c> method replaces all current representations of the target resource
    /// with the request payload.
    /// </summary>
    Put,

    /// <summary>
    /// The <c>DELETE</c> method deletes the specified resource.
    /// </summary>
    Delete,

    /// <summary>
    /// The <c>OPTIONS</c> method describes the communication options
    /// for the target resource.
    /// </summary>
    Options,

    /// <summary>
    /// The <c>PATCH</c> method applies partial modifications to a resource.
    /// </summary>
    Patch
}
