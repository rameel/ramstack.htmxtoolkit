namespace Ramstack.HtmxToolkit;

/// <summary>
/// Defines the HTTP methods that can be used in htmx configuration.
/// </summary>
public enum HttpVerb
{
    /// <summary>
    /// The GET method requests a representation of the specified resource.
    /// </summary>
    Get,

    /// <summary>
    /// The HEAD method asks for a response identical to a GET request, but without the response body.
    /// </summary>
    Head,

    /// <summary>
    /// The POST method submits an entity to the specified resource.
    /// </summary>
    Post,

    /// <summary>
    /// The PUT method replaces all current representations of the target resource with the request payload.
    /// </summary>
    Put,

    /// <summary>
    /// The DELETE method deletes the specified resource.
    /// </summary>
    Delete,

    /// <summary>
    /// The CONNECT method establishes a tunnel to the server identified by the target resource.
    /// </summary>
    Connect,

    /// <summary>
    /// The OPTIONS method describes the communication options for the target resource.
    /// </summary>
    Options,

    /// <summary>
    /// The TRACE method performs a message loop-back test along the path to the target resource.
    /// </summary>
    Trace,

    /// <summary>
    /// The PATCH method applies partial modifications to a resource.
    /// </summary>
    Patch
}
